using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using SWF = System.Windows.Forms;

namespace VcdsDataPlotter.Gui;

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
