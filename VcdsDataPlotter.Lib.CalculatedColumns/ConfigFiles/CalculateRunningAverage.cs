using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VcdsDataPlotter.Lib.CalculatedColumns.ColumnsBuilders;

namespace VcdsDataPlotter.Lib.CalculatedColumns.ConfigFiles;


[XmlType("CalculateRunningAverage")]
public class CalculateRunningAverage : CalculationDefinitionStep
{
    // TODO: This results in a funny format, such as PT1.2S for 1.2 seconds.
    // TODO: Add more running average options when columns become available, such as
    //       running average over a distance
    public TimeSpan? TimeSpan { get; set; }

    public bool ShouldSerializeTimeSpan() => TimeSpan is not null;

    public override ColumnBuilderConfiguration CreateConfiguration(ColumnBuilderConfiguration parent)
    {
        if (TimeSpan is TimeSpan ts)
            return parent.Calculate.RunningChange.Over(ts);
        else
            throw new InvalidOperationException("Timespan to calculate running average over is not specified.");
    }
}
