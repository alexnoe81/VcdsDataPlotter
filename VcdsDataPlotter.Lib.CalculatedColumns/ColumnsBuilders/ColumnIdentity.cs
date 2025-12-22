using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VcdsDataPlotter.Lib.CalculatedColumns.ColumnsBuilders
{
    public class ColumnIdentity
    {
        internal ColumnIdentity(string title, string channelId) => (Title, ChannelId) = (
            title ?? throw new ArgumentNullException(nameof(title)),
            channelId ?? throw new ArgumentNullException(nameof(channelId)));
        public string Title { get; private set; }
        public string ChannelId { get; private set; }

        public override string ToString() => $"[{ChannelId}] {Title}";
    }
}
