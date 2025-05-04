using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.CalculatedColumns
{
    /// <summary>
    /// This column calculates the difference between any row and the first row.
    /// </summary>
    /// This column calculates the difference between any row and the first row. For example, there is a measurement channel
    /// containing the total DEF consumption since the start of time. This column allows determininig the DEF consumption
    /// within the current driving cycle.
    public class DifferenceToFirstLineColumn : CalculatedColumnBase, IDiscreteDataColumn
    {
        private DifferenceToFirstLineColumn(IDiscreteDataColumn sourceColumn, string title)
        {
            this.sourceColumn = sourceColumn ?? throw new ArgumentNullException(nameof(sourceColumn));
            this.Title = title;
        }

        public static DifferenceToFirstLineColumn Create(IDiscreteDataColumn sourceColumn, string title)
        {
            // No initialization needed. All values can be calculated on the fly in EnumerateDataItems
            DifferenceToFirstLineColumn result = new DifferenceToFirstLineColumn(sourceColumn, title);
            return result;
        }

        public string? ChannelId => sourceColumn.ChannelId + "DELTA";

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
    }
}
