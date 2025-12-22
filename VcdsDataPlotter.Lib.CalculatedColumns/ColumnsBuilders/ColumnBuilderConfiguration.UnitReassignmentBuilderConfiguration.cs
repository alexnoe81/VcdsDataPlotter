using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.CalculatedColumns.Columns;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.CalculatedColumns.ColumnsBuilders
{
    public partial class ColumnBuilderConfiguration
    {
        private class UnitReassignmentBuilderConfiguration : ColumnBuilderConfiguration
        {
            public UnitReassignmentBuilderConfiguration(ColumnBuilderConfiguration parent, string targetUnit) => (this.targetUnit, this.parentBuilderConfiguration) = (
                targetUnit ?? throw new ArgumentNullException(nameof(targetUnit)),
                parent ?? throw new ArgumentNullException(nameof(parent)));

            public override ColumnBuilderBuildingResult TryBuild(IList<IDiscreteDataColumn> sourceColumns, [NotNullWhen(true)] out IDiscreteDataColumn? result)
            {
                if (parentBuilderConfiguration.TryBuild(sourceColumns, out var parentColumn) is ColumnBuilderBuildingResult { Success: false } parentBuildResult)
                {
                    result = null;
                    return ColumnBuilderBuildingResult.CreateError(new ColumnBuilderBuildFailure(
                        this, "Failed to build source column.", [parentBuildResult.Problem]));
                }

                try
                {
                    result = UnitReassignment.Create(parentColumn!, targetUnit);
                }
                catch (ArgumentException)
                {
                    result = null;
                    return ColumnBuilderBuildingResult.CreateError(new ColumnBuilderBuildFailure(
                        this, $"Unit '{parentColumn!.Unit}' of column '{parentColumn.ChannelId}/{parentColumn.Title}' cannot be converted to target unit '{targetUnit}'."));
                }

                return ColumnBuilderBuildingResult.SuccessfulResult;
            }

            public override string ToString() => $"Convert unit to '{targetUnit}'";

            private string targetUnit;
            private ColumnBuilderConfiguration parentBuilderConfiguration;
        }
    }
}
