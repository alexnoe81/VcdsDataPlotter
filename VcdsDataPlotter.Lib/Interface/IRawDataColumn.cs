using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.Implementation;

namespace VcdsDataPlotter.Lib.Interface
{
    public record class RawDataItem(TimeSpan TimeStamp, string RawData);

    /// <summary>
    /// A raw data column is one that makes no assumption about the internal structure of data. It just
    /// delivers timestamped raw readings.
    /// </summary>
    public interface IRawDataColumn
    {
        public string ChannelId { get; }
        public string Title { get; }
        public string? RawUnit { get; }
        IEnumerable<RawDataItem> EnumerateDataItems();
    }
}
