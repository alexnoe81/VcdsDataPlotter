using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.RawTable.Columnizer;

/// <summary>
/// This represents a column that yields one single value, and where each cell contains just the value
/// </summary>
public class SimpleDiscreteNumericColumn : SingleValueColumnBase
{
    public SimpleDiscreteNumericColumn(IRawDataColumn rawData) : base(rawData)
    {

    }

    public override string ToString() => $"{ChannelId}: {this.Title} [{this.Unit}]";

    public override IEnumerable<SingleDataItem> EnumerateDataItems()
    {
        foreach (var item in RawData.EnumerateDataItems())
        {
            if (double.TryParse(item.RawData, System.Globalization.NumberStyles.Number, DefaultCulture, out var value))
            {
                yield return new SingleDataItem(item.TimeStamp, value);
            }
        }
    }
}
