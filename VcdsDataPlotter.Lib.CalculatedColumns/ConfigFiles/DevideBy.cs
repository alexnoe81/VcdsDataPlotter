using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VcdsDataPlotter.Lib.CalculatedColumns.ColumnsBuilders;

namespace VcdsDataPlotter.Lib.CalculatedColumns.ConfigFiles;


[XmlType("DevideBy")]
public class DevideBy : CalculationDefinitionStep
{
    public double? Denominator { get; set; }
    public bool ShouldSerializeFactor() => Denominator is not null;

    public CalculationDefinition CalculatedSource { get; set; }

    public override ColumnBuilderConfiguration CreateConfiguration(ColumnBuilderConfiguration parent)
    {
        if (Denominator is double f)
        {
            return parent.DevideBy(f);
        }
        else if (CalculatedSource is CalculationDefinition source)
        {
            return parent.DevideBy(source.Build());
        }
        else throw new InvalidOperationException("Operand for DevideBy is missing. Either a constant factor or a column must be specified.");
    }
}
