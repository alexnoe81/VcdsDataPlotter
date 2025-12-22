using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.CalculatedColumns.Columns;
using VcdsDataPlotter.Lib.CalculatedColumns.Interface;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.CalculatedColumns.Columns
{
    public class InterpolatingColumn : InitializableCalculatedColumnBase, IInterpolatingColumn
    {
        private InterpolatingColumn(IDiscreteDataColumn discreteColumn)
        {
            this.discreteColumn = discreteColumn ?? throw new ArgumentNullException(nameof(discreteColumn));
        }

        public static InterpolatingColumn Create(IDiscreteDataColumn discreteColumn)
        {
            InterpolatingColumn result = new InterpolatingColumn(discreteColumn);
            result.Initialize();
            return result;
        }

        protected override void InternalInitialize()
        {
            sourceItems = [.. discreteColumn.EnumerateDataItems()];
            sourceItemsTimeStamps = new TimeSpan[sourceItems.Length];
            for (int j = 0; j < sourceItems.Length; j++)
                sourceItemsTimeStamps[j] = sourceItems[j].TimeStamp;
        }

        public override string? ToString() => discreteColumn.ToString();

        public IEnumerable<SingleDataItem> EnumerateDataItems()
        {
            foreach (var item in discreteColumn.EnumerateDataItems())
                yield return item;
        }

        public SingleDataItem GetValue(TimeSpan timestamp)
        {
            CheckInitialized();
            if (sourceItems is not { Length: > 0 })
                throw new InvalidOperationException("No data available.");
            if (sourceItemsTimeStamps is not { Length: > 0 })
                throw new InvalidOperationException("No data available.");

            if (sourceItems.Length == 1)
                return sourceItems[0];

            // Find close time stamp using binary search
            var index = Array.BinarySearch(sourceItemsTimeStamps, timestamp);

            if (index >= 0)
            {
                // Exact match found
                return sourceItems[index];
            }

            index = (int)(index ^ 0xFFFFFFFF);

            // Now index is the index of the first element that is larger. If no element is larger,
            // index points to (end+1). We won't extrapolate, only interpolate.
            if (index >= sourceItems.Length)
                return sourceItems[sourceItems.Length - 1];
            if (index == 0)
                return sourceItems[0];

            // Do linear interpolation
            var t1 = sourceItemsTimeStamps[index - 1];
            var t2 = sourceItemsTimeStamps[index];
            var f = (timestamp - t1) / (t2 - t1);

            var v1 = Convert.ToDouble(sourceItems[index - 1].RawData);
            var v2 = Convert.ToDouble(sourceItems[index].RawData);
            var result = v1 + f * (v2 - v1);

            return new(timestamp, result);
        }

        private IDiscreteDataColumn discreteColumn;
        private SingleDataItem[]? sourceItems;
        private TimeSpan[]? sourceItemsTimeStamps;

        public string? ChannelId => discreteColumn.ChannelId;

        public string? Title => discreteColumn.Title;

        public string? Unit => discreteColumn.Unit;
    }
}
