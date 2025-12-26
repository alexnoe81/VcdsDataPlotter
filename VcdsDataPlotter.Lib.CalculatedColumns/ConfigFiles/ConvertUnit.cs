using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VcdsDataPlotter.Lib.CalculatedColumns.ColumnsBuilders;

namespace VcdsDataPlotter.Lib.CalculatedColumns.ConfigFiles;


[XmlType("ConvertUnit")]
public class ConvertUnit : CalculationDefinitionStep
{
    public string TargetUnit { get; set; }

    public override ColumnBuilderConfiguration CreateConfiguration(ColumnBuilderConfiguration parent)
    {
        if (TargetUnit is not null)
            return parent.ConvertUnit(TargetUnit);
        else
            throw new InvalidOperationException("Target unit is not specified.");
    }
}
