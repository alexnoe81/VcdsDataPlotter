using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.CalculatedColumns.Columns;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.CalculatedColumns.Math
{
    /// <summary>
    /// This column calculates the difference between any row and the first row.
    /// </summary>
    /// This column calculates the difference between any row and the first row. For example, there is a measurement channel
    /// containing the total DEF consumption since the start of time. This column allows determininig the DEF consumption
    /// within the current driving cycle.
    public class DifferenceToFirstRowColumn : CalculatedColumnBase, IDiscreteDataColumn
    {
        private DifferenceToFirstRowColumn(string title, string channelId, IDiscreteDataColumn sourceColumn)
        {
            this.sourceColumn = sourceColumn ?? throw new ArgumentNullException(nameof(sourceColumn));
            this.channelIdOverride = channelId;
            Title = title;
        }

        public static DifferenceToFirstRowColumn Create(string title, string channelId, IDiscreteDataColumn sourceColumn)
        {
            // No initialization needed. All values can be calculated on the fly in EnumerateDataItems
            DifferenceToFirstRowColumn result = new DifferenceToFirstRowColumn(title, channelId, sourceColumn);
            return result;
        }

        public string? ChannelId => channelIdOverride is not null ? channelIdOverride : sourceColumn.ChannelId + "_DELTA";

        public string? Title { get; private set; }

        public string? Unit => sourceColumn.Unit;

        public IEnumerable<SingleDataItem> EnumerateDataItems()
        {
            bool first = true;
            double firstData = 0.0;
            foreach (var item in sourceColumn.EnumerateDataItems())
            {
                if (first)
                {
                    firstData = item.RawData;
                    first = false;
                }

                yield return new(item.TimeStamp, item.RawData - firstData);
            }
        }

        private IDiscreteDataColumn sourceColumn;
        private string channelIdOverride;
    }
}
