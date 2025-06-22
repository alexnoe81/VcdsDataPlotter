using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.CalculatedColumns.ColumnSpec
{
    internal class InputColumnSpecChannelId : MatchableInputColumnSpec
    {
        public InputColumnSpecChannelId(string channelId) 
        {
            this.channelId = channelId ?? throw new ArgumentNullException(nameof(channelId));
            this.channelIdLower = channelId.ToLowerInvariant();
        }
        
        public override bool Matches(IDiscreteDataColumn comparee)
        {
            _ = comparee ?? throw new ArgumentNullException(nameof(comparee));

            var compareeChannelId = comparee.ChannelId.ToLowerInvariant();
            if (compareeChannelId.StartsWith("loc."))
                compareeChannelId = compareeChannelId.Substring(4).Trim();

            return compareeChannelId == channelIdLower;
        }

        private string channelId;
        private string channelIdLower;
    }
}
