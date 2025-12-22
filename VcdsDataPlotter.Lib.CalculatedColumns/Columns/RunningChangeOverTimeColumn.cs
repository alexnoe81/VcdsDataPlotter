using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.CalculatedColumns.Interface;
using VcdsDataPlotter.Lib.Physics;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.CalculatedColumns.Columns
{
    /// <summary>
    /// This column calculates the difference per second on a column. Normally, it is used on an IntegralByTime column
    /// to calculate an average over an actual column, such as:
    /// 
    /// EGR mass = ∫ EGR mass flow(t) dt
    ///
    ///                           t1+Δt
    /// averaged EGR mass flow = (  ∫ EGR mass flow(t) dt ) / Δt            <--- this one
    ///                            t1
    /// 
    /// exhaust mass = ∫ exhaust mass flow(t) dt
    /// 
    ///                               t1+Δt
    /// averaged exhaust mass flow = (  ∫ exhaust mass flow(t) dt ) / Δt    <--- this one
    ///                                t1
    /// 
    ///                       averaged EGR mass flow 
    /// averaged EGR rate = --------------------------
    ///                     averaged exhaust mass flow
    /// </summary>
    public class RunningChangeOverTimeColumn : InitializableCalculatedColumnBase, IDiscreteDataColumn
    {
        private RunningChangeOverTimeColumn(string title, string channelId, IInterpolatingColumn sourceColumn, TimeSpan duration)
        {
            this.sourceColumn = sourceColumn ?? throw new ArgumentNullException(nameof(sourceColumn));
            Title = title;
            ChannelId = channelId;
            this.duration = duration;
        }

        public static RunningChangeOverTimeColumn Create(string title, string channelId, IDiscreteDataColumn sourceColumn, TimeSpan duration)
        {
            _ = sourceColumn ?? throw new ArgumentNullException(nameof(sourceColumn));

            if (sourceColumn is not IInterpolatingColumn)
            {
                sourceColumn = InterpolatingColumn.Create(sourceColumn);
            }

            // Make sure we convert the input column to /s
            RunningChangeOverTimeColumn result = new RunningChangeOverTimeColumn(
                title: title,
                channelId: channelId,
                sourceColumn: (IInterpolatingColumn)sourceColumn,
                duration: duration);

            result.Unit = sourceColumn.Unit + "/s";
            result.Initialize();
            return result;
        }

        protected override void InternalInitialize()
        {
            List<SingleDataItem> temp = new();
            SingleDataItem? preceding = null;
            double accumulatedValue = 0;
            foreach (var item in sourceColumn.EnumerateDataItems())
            {
                try
                {
                    var interpolatedEarlierValue = sourceColumn.GetValue(item.TimeStamp.Subtract(duration));
                    if (item.TimeStamp.TotalSeconds - interpolatedEarlierValue.TimeStamp.TotalSeconds < 0.01)
                    {
                        // Interpolation returned item. This is probably the first item
                        continue;
                    }

                    temp.Add(new(item.TimeStamp, (item.RawData - interpolatedEarlierValue.RawData) / (item.TimeStamp.TotalSeconds - interpolatedEarlierValue.TimeStamp.TotalSeconds)));
                }
                finally
                {
                    preceding = item;
                }
            }

            dataItems = temp.ToArray();
        }


        public string? ChannelId { get; private set; }

        public string? Title { get; private set; }

        public string? Unit { get; private set; }

        public IEnumerable<SingleDataItem> EnumerateDataItems()
        {
            CheckInitialized();

            foreach (var item in dataItems!)
                yield return item;
        }

        private IInterpolatingColumn sourceColumn;
        private SingleDataItem[]? dataItems;
        private TimeSpan duration;
    }
}
