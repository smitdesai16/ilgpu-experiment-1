namespace CPUvsGPU
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Application.Run(new Form1());
        }

        // Event handlers
        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            // Log or display e.Exception
            MessageBox.Show("UI Thread Error: " + e.Exception.Message + "\n" + e.Exception.StackTrace);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Log or display (Exception)e.ExceptionObject
            MessageBox.Show("Unhandled Error: " + ((Exception)e.ExceptionObject).Message + "\n" + ((Exception)e.ExceptionObject).StackTrace);
        }
    }
}