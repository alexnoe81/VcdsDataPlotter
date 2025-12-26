using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.CalculatedColumns.Columns;
using VcdsDataPlotter.Lib.CalculatedColumns.ColumnSpecs;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.CalculatedColumns.ColumnsBuilders
{
    public partial class ColumnBuilderConfiguration
    {
        /// <summary>
        /// This configuration selects a column from source columns by specified criteria
        /// </summary>
        private class SelectionBuilderConfiguration : ColumnBuilderConfiguration
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="identity"></param>
            /// <param name="specs">List of criteria by which a column is selected. One column is tested by all provided
            /// criteria, then the next column is tested etc. The first successful match wins.
            /// </param>
            /// <exception cref="ArgumentNullException"></exception>
            public SelectionBuilderConfiguration(ColumnIdentity identity, ColumnSpec[] specs)
                : base(identity)
            {
                Specs = new(specs ?? throw new ArgumentNullException(nameof(specs)));
            }

            public override ColumnBuilderBuildingResult TryBuild(IList<IDiscreteDataColumn> columns, [NotNullWhen(true)] out IDiscreteDataColumn? result)
            {
                foreach (var criterion in this.Specs)
                {
                    foreach (var column in columns)
                    {
                        if (criterion.Matches(column))
                        {
                            result = new RenamedColumn(Identity.Title, Identity.ChannelId, column);
                            return ColumnBuilderBuildingResult.SuccessfulResult;
                        }
                    }
                }

                result = null;
                return ColumnBuilderBuildingResult.CreateError(new ColumnBuilderBuildFailure(
                    this, "None of the provided ColumnSpecs matched any of the provided source columns."));
            }

            public override string ToString() => Specs.Count > 1 ? $"Select first of [ {string.Join("; ", Specs)} ]" : $"Select [ {Specs.First()} ]";

            public ReadOnlyCollection<ColumnSpec> Specs { get; private set; }
        }

        /// <summary>
        /// This configuration selects a column from source columns by specified criteria
        /// </summary>
        private class SelectionBuilderConfiguration2 : ColumnBuilderConfiguration
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="identity"></param>
            /// <param name="specs">List of criteria by which a column is selected. One column is tested by all provided
            /// criteria, then the next column is tested etc. The first successful match wins.
            /// </param>
            /// <exception cref="ArgumentNullException"></exception>
            public SelectionBuilderConfiguration2(ColumnIdentity identity, ColumnBuilderConfiguration[] specs)
                : base(identity)
            {
                Specs = new(specs ?? throw new ArgumentNullException(nameof(specs)));
            }

            public override ColumnBuilderBuildingResult TryBuild(IList<IDiscreteDataColumn> columns, [NotNullWhen(true)] out IDiscreteDataColumn? result)
            {
                foreach (var criterion in this.Specs)
                {
            //        foreach (var column in columns)
            //        {
                        if (criterion.TryBuild(columns, out var buildResult))
                        {
                            result = new RenamedColumn(Identity.Title, Identity.ChannelId, buildResult);
                            return ColumnBuilderBuildingResult.SuccessfulResult;
                        }
            //        }
                }

                result = null;
                return ColumnBuilderBuildingResult.CreateError(new ColumnBuilderBuildFailure(
                    this, "None of the provided source columns is buildable."));
            }

            public override string ToString() => $"Select first of [ {string.Join("; ", Specs)} ]";

            public ReadOnlyCollection<ColumnBuilderConfiguration> Specs { get; private set; }
        }
    }
}
