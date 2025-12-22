using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.CalculatedColumns.ColumnsBuilders
{
    public partial class ColumnBuilderConfiguration
    {
        /// <summary>
        /// This configuration class uses a column that was resolved before
        /// </summary>
        private class UseColumnBuilderConfiguration : ColumnBuilderConfiguration
        {
            public UseColumnBuilderConfiguration(IDiscreteDataColumn source) => this.source = source ?? throw new ArgumentNullException(nameof(source));

            public override ColumnBuilderBuildingResult TryBuild(IList<IDiscreteDataColumn> sourceColumns, [NotNullWhen(true)] out IDiscreteDataColumn? result)
            {
                result = source;
                return ColumnBuilderBuildingResult.SuccessfulResult;
            }

            public override string? ToString() => source.ToString();

            private IDiscreteDataColumn source;
        }
    }
}
