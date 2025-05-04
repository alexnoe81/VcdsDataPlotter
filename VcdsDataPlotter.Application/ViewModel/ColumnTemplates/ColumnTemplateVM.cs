using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Gui.ViewModel.Base;
using VcdsDataPlotter.Lib.Implementation.CalculatedColumns.Templates;

namespace VcdsDataPlotter.Gui.ViewModel.ColumnTemplates;
                                                       
public class ColumnTemplateVM : ViewModelBase
{
    public ColumnTemplateVM(IColumnTemplate template)
    {
        this.template = template ?? throw new ArgumentNullException(nameof(template));
        this.name = template.TemplateName;
    }

    public string Name 
    {
        get => this.name;
        set => SetProperty(ref this.name, value);
    }


    private IColumnTemplate template;
    private string name;
}
