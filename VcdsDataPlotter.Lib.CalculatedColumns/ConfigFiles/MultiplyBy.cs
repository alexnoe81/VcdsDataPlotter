using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VcdsDataPlotter.Lib.CalculatedColumns.ColumnsBuilders;

namespace VcdsDataPlotter.Lib.CalculatedColumns.ConfigFiles;


[XmlType("MultiplyBy")]
public class MultiplyBy : CalculationDefinitionStep
{
    public double? Factor { get; set; }
    public bool ShouldSerializeFactor() => Factor is not null;

    public CalculationDefinition CalculatedSource { get; set; }

    public override ColumnBuilderConfiguration CreateConfiguration(ColumnBuilderConfiguration parent)
    {
        if (Factor is double f)
        {
            return parent.MultiplyBy(f);
        }
        else if (CalculatedSource is CalculationDefinition source)
        {
            return parent.MultiplyBy(source.Build());
        }
        else throw new InvalidOperationException("Operand for MultiplyBy is missing. Either a constant factor or a column must be specified.");
    }
}
