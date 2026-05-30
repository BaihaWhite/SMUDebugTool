using System;
using System.Windows.Forms;

namespace ZenStatesDebugTool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += ApplicationThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            try
            {
                Form MainForm = new SettingsForm();
                string appString = $"{Application.ProductName} {Application.ProductVersion.Substring(0, Application.ProductVersion.LastIndexOf('.'))}";
#if DEBUG
                appString += " (debug)";
#endif
                MainForm.Text = appString;
                Application.Run(MainForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"应用程序遇到未处理的错误。\n\n{ex.GetType().Name}: {ex.Message}", "致命错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        static void ApplicationThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            MessageBox.Show($"发生致命错误，应用程序必须关闭。\n\n{ex?.GetType().Name ?? "Unknown"}: {ex?.Message ?? "无详细信息"}", "致命错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
