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

namespace VcdsDataPlotter.Gui.View
{
    /// <summary>
    /// Interaction logic for DocumentAdvancedView.xaml
    /// </summary>
    /// A DocumentAdvancedView does not only show or hide existing columns, but allows the user to
    /// configure the display in a much more detailed way.
    public partial class DocumentAdvancedPresentation : UserControl
    {
        public DocumentAdvancedPresentation()
        {
            InitializeComponent();
        }
    }
}
