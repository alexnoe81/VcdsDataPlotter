using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.CalculatedColumns.Columns;
using VcdsDataPlotter.Lib.CalculatedColumns.Math;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.CalculatedColumns.ColumnsBuilders
{
    /// <summary>
    /// A ColumnBuilderConfiguration is used to create a description of a column that can be materialized
    /// from a list of input solumns
    /// </summary>
    public partial class ColumnBuilderConfiguration
    {
        private class ForCalculation : ChainElement, IColumnBuilderForCalculation
        {
            public ForCalculation(ColumnBuilderConfiguration parentBuilderConfiguration, ColumnIdentity identity)
                : base(parentBuilderConfiguration, identity) { }

            /// <summary>
            /// Probably not needed, because we can call IntegrateByTime on a ColumnBuilderConfiguration
            /// </summary>
            public IIntegralByTimeContainer IntegralByTime { get => new IntegralByTimeContainer(ParentBuilderConfiguration, Identity); }
            
            /// <summary>
            /// Creates a container for a running average calculation. The container has function Over(TimeSpan), allowing
            /// to specify the time span for the running average.
            /// </summary>
            public IRunningChangeContainer RunningChange { get => new RunningChangeContainer(ParentBuilderConfiguration, Identity); }

            /// <summary>
            /// Creates a ColumnBuilderConfiguration that calculates the difference of each row to the first row. This
            /// is useful if a column contains the accumulated data of something over the life span of a vehicle, such
            /// as the total Fuel or DEF consumption.
            /// </summary>
            public ColumnBuilderConfiguration DifferenceToFirstRow { get => new DifferenceToFirstRowBuilderConfiguration(Identity, ParentBuilderConfiguration); }

            /// <summary>
            /// Used to build integals by time. The caller must provide the column to be integrated by time 
            /// to the Over() function
            /// </summary>
            private class IntegralByTimeContainer : ChainElement, IIntegralByTimeContainer
            {
                public IntegralByTimeContainer(ColumnBuilderConfiguration parentBuilderConfiguration, ColumnIdentity identity)
                    : base(parentBuilderConfiguration, identity) { }

                public ColumnBuilderConfiguration Over(ColumnBuilderConfiguration sourceColumnSelector) => new IntegrateByTimeConfigurationBuilder(Identity, sourceColumnSelector);
            }

            private class RunningChangeContainer : ChainElement, IRunningChangeContainer
            {
                public RunningChangeContainer(ColumnBuilderConfiguration parentBuilderConfiguration, ColumnIdentity identity)
                    : base(parentBuilderConfiguration, identity) { }

                public ColumnBuilderConfiguration Over(TimeSpan duration) => new ChangeOverTime(duration, Identity, ParentBuilderConfiguration);

                private class ChangeOverTime : CalculatedColumnBuilderConfiguration
                {
                    public ChangeOverTime(TimeSpan duration, ColumnIdentity identity, ColumnBuilderConfiguration sourceColumnBuilderConfiguration)
                        : base(identity, [sourceColumnBuilderConfiguration])
                    {
                        this.duration = duration;
                    }

                    public override ColumnBuilderBuildingResult TryBuild(IList<IDiscreteDataColumn> sourceColumns, [NotNullWhen(true)] out IDiscreteDataColumn? result)
                    {
                        var success = TryResolveSourceColumns(sourceColumns, out var resolvedSourceColumns, out var problems);
                        if (!success)
                        {
                            result = null;
                            return ColumnBuilderBuildingResult.CreateError(new(this, "Failed to resolve source column.", problems));
                        }

                        var sourceColumn = resolvedSourceColumns.First();
                        result = RunningChangeOverTimeColumn.Create(Identity.Title, Identity.ChannelId, sourceColumn, duration);

                        return ColumnBuilderBuildingResult.SuccessfulResult;
                    }

                    public override string ToString() => $"Running average over {duration} of '{SourceColumnBuilderConfigurations.First()}'";

                    private TimeSpan duration;
                }
            }

            private class DifferenceToFirstRowBuilderConfiguration : CalculatedColumnBuilderConfiguration
            {
                public DifferenceToFirstRowBuilderConfiguration(
                    ColumnIdentity identity,
                    ColumnBuilderConfiguration sourceColumnBuilderConfiguration)
                 : base(identity, [sourceColumnBuilderConfiguration])
                {
                }

                public override ColumnBuilderBuildingResult TryBuild(IList<IDiscreteDataColumn> sourceColumns, [NotNullWhen(true)] out IDiscreteDataColumn? result)
                {
                    var success = TryResolveSourceColumns(sourceColumns, out var resolvedSourceColumns, out var problems);
                    if (!success)
                    {
                        result = null;
                        return ColumnBuilderBuildingResult.CreateError(new(this, "Failed to resolve source column.", problems));
                    }

                    var sourceColumn = resolvedSourceColumns.First();
                    result = DifferenceToFirstRowColumn.Create(Identity.Title, Identity.ChannelId, sourceColumn);

                    return ColumnBuilderBuildingResult.SuccessfulResult;
                }

                public override string ToString() => $"Difference to firth row w of '{SourceColumnBuilderConfigurations.First()}'";
            }
        }
    }
}
