﻿using System;
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
using VcdsDataPlotter.Lib.Implementation.CalculatedColumns.Templates;

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

            foreach (var item in currentSelection.OfType<SourceColumnVM>())
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

        private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is DocumentVM old)
            {
                old.OnCmdAddColumn -= HandleCmdAddColumn;
            }

            if (e.NewValue is DocumentVM @new)
            {
                @new.OnCmdAddColumn += HandleCmdAddColumn;
            }
        }

        private void HandleCmdAddColumn(object? sender, EventArgs e)
        {
            var vm = this.DataContext as DocumentVM;

            var createColumnVm = new CreateVirtualColumnWindowVM();

            List<IColumnTemplate> templates = [.. templateFactory.GetTemplates()];
            foreach (var item in templates)
                createColumnVm.Templates.Add(new(item));

            foreach (var item in vm.SourceColumns)
                createColumnVm.AllAvailableColumns.Add(item);
            foreach (var item in vm.CalculatedColumns)
                createColumnVm.AllAvailableColumns.Add(item);

            CreateVirtualColumnWindow window = new CreateVirtualColumnWindow();
            window.DataContext = createColumnVm;
            window.Show();
        }

        private TemplateFactory templateFactory = new TemplateFactory();
    }
}
