using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VcdsDataPlotter.Lib.Implementation.CalculatedColumns.Templates;

/// <summary>
/// A template factory is used to retrieve all templates from which calculated columns can
/// be instantiated.
/// </summary>
public class TemplateFactory
{
    public TemplateFactory()
    {
        allTemplatesCollection = new ReadOnlyCollection<IColumnTemplate>(new List<IColumnTemplate>
        {
            new ColumnTemplateIntegralByTime()
        });
    }

    /// <summary>
    /// Gets all column templates
    /// </summary>
    /// <returns></returns>
    public IReadOnlyCollection<IColumnTemplate> GetTemplates()  => allTemplatesCollection;

    private IReadOnlyCollection<IColumnTemplate> allTemplatesCollection;
}
