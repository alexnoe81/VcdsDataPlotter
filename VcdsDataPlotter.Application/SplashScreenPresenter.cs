using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using SWF = System.Windows.Forms;

namespace VcdsDataPlotter.Gui;

static class SplashScreenPresenter
{
    public static void ShowSplashScreen()
    {
        ManualResetEventSlim splashScreenVisibleEvent = new ManualResetEventSlim(false);

        Thread splashScreenThread = new Thread(new ThreadStart(() =>
        {
            SWF.Application.EnableVisualStyles();
            SWF.Application.SetHighDpiMode(SWF.HighDpiMode.SystemAware);
            SplashScreen splashScreen = new SplashScreen();
            splashScreen.Show();

            splashScreenVisibleEvent.Set();
            SWF.Application.Run();
        }));
        splashScreenThread.SetApartmentState(ApartmentState.STA);
        splashScreenThread.Start();
        splashScreenVisibleEvent.Wait();
    }
}
