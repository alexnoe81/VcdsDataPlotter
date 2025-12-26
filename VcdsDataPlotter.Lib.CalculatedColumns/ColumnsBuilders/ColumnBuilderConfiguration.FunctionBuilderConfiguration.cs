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
        private class FunctionBuilderConfiguration : CalculatedColumnBuilderConfiguration
        {
            public FunctionBuilderConfiguration(
                ColumnIdentity identity,
                ColumnBuilderConfiguration sourceColumnBuilderConfiguration,
                ColumnBuilderConfiguration otherBuilderConfiguration,
                Func<double, double, double?> @operator, 
                string toStringTemplate)
                 : base(identity, [sourceColumnBuilderConfiguration, otherBuilderConfiguration])
            {
                this.@operator = @operator ?? throw new ArgumentNullException(nameof(@operator));
                this.toStringTemplate = toStringTemplate ?? "?";
            }

            public override ColumnBuilderBuildingResult TryBuild(IList<IDiscreteDataColumn> sourceColumns, [NotNullWhen(true)] out IDiscreteDataColumn? result)
            {
                var success = TryResolveSourceColumns(sourceColumns, out var resolvedSourceColumns, out var problems);
                if (!success)
                {
                    result = null;
                    return ColumnBuilderBuildingResult.CreateError(new(this, "Failed to resolve source column.", problems));
                }

                result = MultiColumnCalculation.Create(Identity.Title, Identity.ChannelId, resolvedSourceColumns.ElementAt(0), resolvedSourceColumns.ElementAt(1), @operator);
                return ColumnBuilderBuildingResult.SuccessfulResult;
            }

            public override string ToString() => string.Format(toStringTemplate, SourceColumnBuilderConfigurations.Cast<object>().ToArray());

            private Func<double, double, double?> @operator;
            private string toStringTemplate;
        }
    }
}
