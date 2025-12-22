using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VcdsDataPlotter.Lib.RawTable.Columnizer.Interface
{
    public record class SingleDataItem(TimeSpan TimeStamp, double RawData)
    {
        public override string ToString() => ToString(CultureInfo.CurrentCulture);
        public string ToString(CultureInfo cultureInfo) => string.Format(cultureInfo, "[{0:hh\\:mm\\:ss\\.fff}]: {1}", TimeStamp, RawData);
    }

    public class SingleDataItemComparerByTimestamp : IComparer<SingleDataItem>
    {
        public int Compare(SingleDataItem? x, SingleDataItem? y)
        {
            if (x is null && y is null)
                return 0;
            if (x is null && y is not null)
                return -1;
            if (x is not null && y is null)
                return +1;
            return x!.TimeStamp.CompareTo(y!.TimeStamp);
        }
    }

    /// <summary>
    /// A discrete data column is one that returns discrete values that are present
    /// in the source file
    /// </summary>
    public interface IDiscreteDataColumn
    {
        string? ChannelId { get; }
        string? Title { get; }

        // TODO: Think about how to handle simple scalings, like ppm
        string? Unit { get; }

        IEnumerable<SingleDataItem> EnumerateDataItems();
    }
}
