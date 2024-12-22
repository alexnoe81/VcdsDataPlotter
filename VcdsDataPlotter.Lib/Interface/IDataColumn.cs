using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VcdsDataPlotter.Lib.Interface
{
    public record class SingleDataItem(TimeSpan TimeStamp, double RawData);
    public record class SingleRawDataItem(TimeSpan TimeStamp, string RawData);

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
            return x.TimeStamp.CompareTo(y.TimeStamp);
        }
    }

    public interface IDiscreteDataColumn
    {
        string? ChannelId { get; }
        string? Title { get; }
        string? Unit { get; }

        IEnumerable<SingleDataItem> EnumerateDataItems();
    }

    public interface IInterpolatingColumn
    {
        string? ChannelId { get; }
        string? Title { get; }
        string? Unit { get; }

        IEnumerable<SingleDataItem> EnumerateDataItems();
        
        object GetValue(TimeSpan timestamp);
    }
}
