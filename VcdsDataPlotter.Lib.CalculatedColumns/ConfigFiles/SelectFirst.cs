using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VcdsDataPlotter.Lib.CalculatedColumns.ColumnsBuilders;

namespace VcdsDataPlotter.Lib.CalculatedColumns.ConfigFiles;


[XmlType("SelectFirst")]
public class SelectFirst : CalculationDefinitionStep
{
    [XmlArray("Choices")]
    [XmlArrayItem("Choice")]
    public CalculationDefinition[] Choices { get; set; }

    public override ColumnBuilderConfiguration CreateConfiguration(ColumnBuilderConfiguration parent)
    {
        return parent.SelectFirst(Choices.Select(x => x.Build()).ToArray());
    }
}
