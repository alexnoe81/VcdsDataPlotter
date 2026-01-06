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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VcdsDataPlotter.Gui.ViewModel;

namespace VcdsDataPlotter.Gui.View
{
    /// <summary>
    /// Interaction logic for DocumentRawPresentation.xaml
    /// </summary>
    /// A DocumentRawPresentation allows access to the raw data, meaning that it displays data tables
    public partial class DocumentRawPresentation : UserControl
    {
        public DocumentRawPresentation()
        {
            InitializeComponent();
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var vm = DataContext as DocumentRawPresentationVM;

            if (e.Column is DataGridTextColumn tc)
            {
                if (tc.Header is string headerAsString)
                {
                    // This is somewhat hackish. We are bound to a DataTable, and a DataTable uses the column name as header text
                    var settings = vm.ColumnsDisplaySettings[headerAsString];
                    tc.Binding.StringFormat = settings.RecommendedStringFormat;
                    tc.Header = settings.DisplayName;
                }


                var hdr = tc.Header;
            }
        }


    }
}
