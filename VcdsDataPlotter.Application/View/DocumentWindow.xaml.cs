using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VcdsDataPlotter.Gui.ViewModel;

namespace VcdsDataPlotter.Gui.View
{
    /// <summary>
    /// Interaction logic for DocumentWindow.xaml
    /// </summary>
    public partial class DocumentWindow : Window
    {
        public DocumentWindow()
        {
            InitializeComponent();
        }

        private void availableSourceColumns_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var currentSelection = availableSourceColumns.SelectedItems;
            plotControl.Plot.Clear();

            foreach (var item in currentSelection.OfType<DataColumnVM>())
            {
                var xs = item.EnumeratePoints().Select(x => x.TimeStamp.TotalSeconds).ToArray();
                var ys = item.EnumeratePoints().Select(x => x.RawData).ToArray();
                if (!xs.Any())
                    continue;

                var line = plotControl.Plot.Add.ScatterLine(xs, ys);
                line.LegendText = item.SourceColumnId + ": " + item.SourceColumnTitle ?? "";
            }

            plotControl.Plot.ScaleFactor = 2;
            plotControl.Plot.XLabel("Time [s]");
            plotControl.Plot.Axes.AutoScale();
            plotControl.Refresh();
        }
    }
}
