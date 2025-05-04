using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VcdsDataPlotter.Lib.Implementation.CalculatedColumns.Templates;


/// <summary>
/// Within a column template, an IInputColumnSpecification defines requirements a possible
/// column must meet to serve as input column for the templated column
/// </summary>
public interface IInputColumnSpecification
{
    string Explanation { get; }
    
    // TODO: Find better way to describe this. In the end, it must be something like "mass flow", "mass per time" etc.
    string ExplanationForUnit { get; }
}

public interface IColumnTemplate
{
    IInputColumnSpecification[] InputColumnSpecifications { get; }
    CalculatedColumnArgumentTemplate[] ArgumentTemplates { get; }
    public string TemplateName { get; set; }
}

/// <summary>
/// A column template is something that can be displayed in the GUI when adding a new calculated column.
/// </summary>
/// - The ViewModel must contain a list of all column templates
/// - A calculated column must use at least one column as input, otherwise it would not be anm actually calculated column
public abstract class ColumnTemplateBase : IColumnTemplate
{
    protected ColumnTemplateBase()
    {
    }

    public CalculatedColumnArgumentTemplate[] ArgumentTemplates { get; set; }

    public IInputColumnSpecification[] InputColumnSpecifications { get; set; }
    public string TemplateName { get; set; }
}
