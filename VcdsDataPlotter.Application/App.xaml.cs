using Serilog;
using System;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;

namespace VcdsDataPlotter.Gui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public void Application_Startup(object sender, StartupEventArgs e)
        {
            this.DispatcherUnhandledException += HandleDispatcherUnhandledException;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            this.DispatcherUnhandledException -= HandleDispatcherUnhandledException;
        }

        private void HandleDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var localLogger = ClassLogger.ForMember();

            localLogger.Fatal(e.Exception, "Unhandled exception in dispatcher thread: {exceptionMessage}", e.Exception.Message);
            e.Handled = false;
        }

        private static ILogger ClassLogger = Serilog.Log.Logger.ForClass(typeof(App));
    }
}
