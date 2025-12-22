using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.CalculatedColumns.ColumnsBuilders;

public partial class ColumnBuilderConfiguration
{
    /// <summary>
    /// Base class for configurations that represent a calculated column
    /// </summary>
    public class CalculatedColumnBuilderConfiguration : ColumnBuilderConfiguration
    {
        protected CalculatedColumnBuilderConfiguration(ColumnIdentity identity, IList<ColumnBuilderConfiguration> sourceColumnBuilderConfigurations)
            : base(identity)
        {
            this.sourceColumnBuilderConfigurations = new(sourceColumnBuilderConfigurations ?? throw new ArgumentNullException(nameof(sourceColumnBuilderConfigurations)));
        }

        protected bool TryResolveSourceColumns(
            IList<IDiscreteDataColumn> sourceColumns,
            [NotNullWhen(true)] out IList<IDiscreteDataColumn>? result,
            [NotNullWhen(false)] out IList<ColumnBuilderBuildFailure>? failures)
        {
            var resolvedColumns = new List<IDiscreteDataColumn>();
            List<ColumnBuilderBuildFailure> innerFailures = new();
            foreach (var sourceColumnConfiguration in sourceColumnBuilderConfigurations)
            {
                if (sourceColumnConfiguration.TryBuild(sourceColumns, out var resolvedColumn) is ColumnBuilderBuildingResult detailedResult)
                {
                    if (detailedResult.Success)
                    {
                        resolvedColumns.Add(resolvedColumn);
                    }
                    else
                    {
                        innerFailures.Add(detailedResult.Problem);
                    }
                }
            }

            result = resolvedColumns;

            // Sometimes, the caller might need all columns in sourceColumnBuilderConfigurations, sometimes not?, 
            if (innerFailures.Count == 0)
            {
                failures = null;
                return true;
            }
            else
            {
                failures = innerFailures;
                return false;
            }
        }

        protected IReadOnlyCollection<ColumnBuilderConfiguration> SourceColumnBuilderConfigurations => sourceColumnBuilderConfigurations;

        private ReadOnlyCollection<ColumnBuilderConfiguration> sourceColumnBuilderConfigurations;
    }

}
