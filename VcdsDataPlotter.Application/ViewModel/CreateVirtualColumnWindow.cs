using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Gui.ViewModel.Base;
using VcdsDataPlotter.Gui.ViewModel.ColumnTemplates;

namespace VcdsDataPlotter.Gui.ViewModel
{
    public class CreateVirtualColumnWindowVM : ViewModelBase
    {
        public ColumnTemplateVM? SelectedColumnTemplate
        {
            get => this.selectedColumnTemplate;
            set => SetProperty(ref this.selectedColumnTemplate, value);
        }




        public ObservableCollection<ColumnTemplateVM> Templates => this.templates;
        public ObservableCollection<DataColumnVM> AllAvailableColumns => this.allAvailableColumns;


        private ObservableCollection<ColumnTemplateVM> templates = new ObservableCollection<ColumnTemplateVM>();
        private ObservableCollection<DataColumnVM> allAvailableColumns = new ObservableCollection<DataColumnVM>();
        private ColumnTemplateVM? selectedColumnTemplate;
    }
}
