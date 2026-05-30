# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run

- **Build (Debug)**: Open `ZenStatesDebugTool.sln` in Visual Studio and build, or run `msbuild ZenStatesDebugTool.csproj /p:Configuration=Debug`
- **Build (Release)**: `msbuild ZenStatesDebugTool.csproj /p:Configuration=Release`
- **Target**: .NET Framework 4.5, Windows Forms (WinExe)
- **Dependencies**: Restored via NuGet (`packages.config`): Newtonsoft.Json 13.0.3, TaskScheduler 2.10.1
- **Prebuilt dependency**: `Prebuilt/ZenStates-Core.dll` — the core SMU/PCI/MSR I/O library (no source in this repo)
- **Permission**: Must run as Administrator to access hardware registers
- **Output**: `bin/Debug/SMUDebugTool.exe` or `bin/Release/SMUDebugTool.exe`

## Project Architecture

### Entry Point
- `Program.cs` — STAThread entry, creates `SettingsForm` as main window

### Main Form (SettingsForm.cs)
The entire application is driven from `SettingsForm`, a tabbed WinForms UI with these tabs:
- **Info** — System info display (CPU name, SMU version, BIOS, etc.)
- **SMU CMD** — Raw SMU mailbox command interface (read/write/scan)
- **PCI** — PCI config space read/write/scan
- **MSR** — Model-Specific Register read/write/scan
- **CPUID** — CPUID instruction read/scan/decode
- **P-State** — P-State FID/DID read/write
- **PBO** — Precision Boost Overdrive / Curve Optimizer margin control, FMax, startup task
- **BCLK** — Base clock adjustment
- **Power Table** — Real-time power table value monitoring
- **SMU Debug** — SMU mailbox traffic monitor
- **WMI** — AMD ACPI WMI command interface for BIOS settings
- **Core Control** — Core disable/enable via downcore + SMT
- **Memory Dump** — Physical memory dumping tool

### Sub-Forms
- `PowerTableMonitor.cs` — Timer-driven float[] display from `Cpu.powerTable.Table` (2s interval)
- `SMUMonitor.cs` — Real-time SMU command/response traffic logger (10ms poll)
- `PCIRangeMonitor.cs` — PCI address range change monitor (500ms poll, highlights changed rows)
- `ResultForm.cs` — Generic result display with save-to-file support

### Utilities (`Utils/`)
- `SmuAddressSet.cs` — Data class (MsgAddress, RspAddress, ArgAddress)
- `MailboxListItem.cs` — ComboBox item wrapping SMU mailbox addresses
- `CoreListItem.cs` — ComboBox item for core selection (CCD/CCX/CORE)
- `FrequencyListItem.cs` — ComboBox item for frequency multiplier display
- `WmiCmdListItem.cs` — ComboBox item wrapping WMI command ID/value
- `NUMAUtil.cs` — P/Invoke for NUMA node detection and thread group affinity

### Key Dependencies
- **ZenStates-Core.dll** (`Prebuilt/`) — Provides `Cpu`, `SMU`, `Mailbox`, `IOModule` classes and SMU status/command enums
- `CpuSingleton.cs` — Lazy singleton wrapping `ZenStates.Core.Cpu` for global access (used by `MemoryDumper`)

### Supported CPU Families
Handles AMD Ryzen/EPYC families by SMU address ranges: BristolRidge, RavenRidge/Picasso, PinnacleRidge/SummitRidge, Matisse, Vermeer, Raphael, GraniteRidge, Rome, and others.
