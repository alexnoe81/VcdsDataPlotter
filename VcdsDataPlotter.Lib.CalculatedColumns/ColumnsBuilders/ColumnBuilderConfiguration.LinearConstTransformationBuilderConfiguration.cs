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
		private class LinearConstTransformationBuilderConfiguration : CalculatedColumnBuilderConfiguration
		{
			public LinearConstTransformationBuilderConfiguration(ColumnIdentity identity, ColumnBuilderConfiguration sourceColumnBuilderConfiguration, double factor, double offset)
					: base(identity, [sourceColumnBuilderConfiguration])
			{
				this.factor = factor;
				this.offset = offset;
			}

			public override ColumnBuilderBuildingResult TryBuild(IList<IDiscreteDataColumn> sourceColumns, [NotNullWhen(true)] out IDiscreteDataColumn? result)
			{
				var success = TryResolveSourceColumns(sourceColumns, out var resolvedSourceColumns, out var problems);
				if (!success)
				{
					result = null;
					return ColumnBuilderBuildingResult.CreateError(new(this, "Failed to resolve source column.", problems.First().Reasons));
				}

				var sourceColumn = resolvedSourceColumns.First();
				result = LinearConstTransformation.Create(Identity.Title, Identity.ChannelId, sourceColumn, factor: factor, offset: offset);

				return ColumnBuilderBuildingResult.SuccessfulResult;
			}

			public override string ToString() => $"Linear transformation '{factor} x + {offset}' of '{SourceColumnBuilderConfigurations.First()}'";


			private double factor;
			private double offset;
		}
	}
}
