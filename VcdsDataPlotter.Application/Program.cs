using System;

namespace VcdsDataPlotter.Gui;

// <a href="https://www.flaticon.com/free-icons/scatter-plot" title="scatter plot icons">Scatter plot icons created by designhub - Flaticon</a>
static class Program
{
    [STAThread]
    static int Main(string[] args)
    {
        SplashScreenPresenter.ShowSplashScreen();

        try
        {
            SerilogInitializer.Initialize();
            RunWpfApp();

            return 0;
        }
        finally
        {
            Serilog.Log.CloseAndFlush();
        }
    }

    static void RunWpfApp()
    {
        App app = new App();
        app.InitializeComponent();
        app.Run();
    }
}
