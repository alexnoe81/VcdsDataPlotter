using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VcdsDataPlotter.Lib.CalculatedColumns.ColumnsBuilders;
using VcdsDataPlotter.Lib.CalculatedColumns.ColumnSpecs;

namespace VcdsDataPlotter.Lib.CalculatedColumns.ConfigFiles;


[XmlType("Select")]
public class Select : CalculationDefinitionStep
{
    public string ChannelId { get; set; }
    public CalculationDefinition CalculatedSource { get; set; }

    public override ColumnBuilderConfiguration CreateConfiguration(ColumnBuilderConfiguration parent)
    {
        return parent.Select(ColumnSpec.ChannelIdIs(ChannelId));
    }
}
