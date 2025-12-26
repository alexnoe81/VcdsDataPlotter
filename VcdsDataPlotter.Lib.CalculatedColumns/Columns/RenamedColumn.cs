using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.CalculatedColumns.Columns
{
    public class RenamedColumn : CalculatedColumnBase, IDiscreteDataColumn
    {
        public RenamedColumn(string title, string channelId, IDiscreteDataColumn source)
        {
            this.source = source;
            Title = title;
            ChannelId = channelId;
        }

        public string? ChannelId { get; set; }

        public string? Title { get; set; }

        public string? Unit => source.Unit;

        public IEnumerable<SingleDataItem> EnumerateDataItems()
        {
            foreach (var item in source.EnumerateDataItems())
                yield return item;
        }

        private IDiscreteDataColumn source;
    }
}
