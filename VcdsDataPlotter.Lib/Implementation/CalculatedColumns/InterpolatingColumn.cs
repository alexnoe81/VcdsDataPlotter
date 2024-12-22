using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.Interface;

namespace VcdsDataPlotter.Lib.Implementation.CalculatedColumns
{
    public class InterpolatingColumn : IInterpolatingColumn
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

        public void Initialize()
        {
            sourceItems = [.. discreteColumn.EnumerateDataItems()];
            sourceItemsTimeStamps = new TimeSpan[sourceItems.Length];
            for (int j=0; j<sourceItems.Length;j++)
                sourceItemsTimeStamps[j] = sourceItems[j].TimeStamp;
        }

        public IEnumerable<SingleDataItem> EnumerateDataItems()
        {
            foreach (var item in discreteColumn.EnumerateDataItems())
                yield return item;
        }

        public object GetValue(TimeSpan timestamp)
        {
            if (sourceItems.Length == 0)
                throw new InvalidOperationException("???");
            if (sourceItems.Length == 1)
                return sourceItems[0].RawData;

            // Find close time stamp using binary search
            var index = Array.BinarySearch(this.sourceItemsTimeStamps, timestamp);

            if (index >= 0)
            {
                // Exact match found
                return sourceItems[index].RawData;
            }

            index = (int)(index ^ 0xFFFFFFFF);

            // Now index is the index of the first element that is larger. If no element is larger,
            // index points to (end+1). We won't extrapolate, only interpolate.
            if (index >= this.sourceItems.Length)
                return this.sourceItems[this.sourceItems.Length - 1].RawData;
            if (index == 0)
                return this.sourceItems[0].RawData;

            // Do linear interpolation
            var t1 = this.sourceItemsTimeStamps[index - 1];
            var t2 = this.sourceItemsTimeStamps[index];
            var f = (timestamp - t1) / (t2 - t1);

            var v1 = Convert.ToDouble(this.sourceItems[index-1].RawData);
            var v2 = Convert.ToDouble(this.sourceItems[index].RawData);
            var result = v1 + f * (v2 - v1);

            return result;
        }

        private IDiscreteDataColumn discreteColumn;
        private SingleDataItem[] sourceItems;
        private TimeSpan[] sourceItemsTimeStamps;

        public string? ChannelId => this.discreteColumn.ChannelId;

        public string? Title => this.discreteColumn.Title;

        public string? Unit => this.discreteColumn.Unit;
    }
}
