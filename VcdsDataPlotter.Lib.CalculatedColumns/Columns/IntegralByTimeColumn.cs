using System;
using VcdsDataPlotter.Lib.CalculatedColumns.Math;
using VcdsDataPlotter.Lib.Physics;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.CalculatedColumns.Columns
{
    /// <summary>
    /// Represents a calculated column containing ∫ sourceColumn(t) dt
    /// </summary>
    /// TODO: We might need to lift the restriction about only integrating stuff over time if stuff is by time,
    /// because if we want to average a temperature over a few seconds, it still makes sense to calculate the
    /// integral over the temperature by time.
    public class IntegralByTimeColumn : InitializableCalculatedColumnBase, IDiscreteDataColumn
    {
        private IntegralByTimeColumn(string title, string channelId, IDiscreteDataColumn sourceColumn)
        {
            this.sourceColumn = sourceColumn ?? throw new ArgumentNullException(nameof(sourceColumn));
            Title = title;
            ChannelId = channelId;
        }

        /// <summary>
        /// Creates a column that contains ∫ sourceColumn(t) dt
        /// </summary>
        /// <param name="sourceColumn">The column to integrate over</param>
        /// <param name="title">Title of the virtual column</param>
        /// <param name="channelId">Virtual measurement channel Id</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static IntegralByTimeColumn Create(string title, string channelId, IDiscreteDataColumn sourceColumn)
        {
            _ = sourceColumn ?? throw new ArgumentNullException(nameof(sourceColumn));

            var deconstructedUnit = UnitHelpers.DeconstructUnit(sourceColumn.Unit);
            if (deconstructedUnit.Denominator is not { Length: > 0 } || UnitHelpers.GetBaseUnit(deconstructedUnit.Denominator) != BaseUnits.Time)
                throw new ArgumentException($"Source column cannot be integrated by time: {sourceColumn}.", nameof(sourceColumn));

            // Make sure we convert the input column to /s
            IntegralByTimeColumn result = new IntegralByTimeColumn(
                title: title, 
                channelId: channelId,
                sourceColumn: deconstructedUnit.Denominator == BaseUnits.Time
                    ? sourceColumn 
                    : UnitTransformation.Create(sourceColumn, deconstructedUnit.Nominator + "/" + BaseUnits.Time));
            result.Unit = deconstructedUnit.Nominator;
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
                    if (preceding is not null)
                    {
                        double timeDifference = item.TimeStamp.TotalSeconds - preceding.TimeStamp.TotalSeconds;
                        if (timeDifference < 0.01)
                        {
                            // Two consecutive items have the same time stamp. Ignore this one
                            continue;
                        }

                        double avgValue = (item.RawData + preceding.RawData) / 2;
                        double additionalArea = timeDifference * avgValue;
                        accumulatedValue += additionalArea;
                        temp.Add(new(item.TimeStamp, accumulatedValue));
                    }
                    else
                    {
                        temp.Add(new(item.TimeStamp, accumulatedValue));
                    }
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

        private IDiscreteDataColumn sourceColumn;
        private SingleDataItem[]? dataItems;
    }
}
