using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.CalculatedColumns.Columns;
using VcdsDataPlotter.Lib.CalculatedColumns.ColumnSpecs;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.CalculatedColumns.ColumnsBuilders
{
    /// <summary>
    /// A ColumnBuilderConfiguration is used to create a description of a column that can be materialized
    /// from a list of input solumns
    /// </summary>
    public partial class ColumnBuilderConfiguration
    {
        private ColumnBuilderConfiguration(string title, string channelId) => Identity = new ColumnIdentity(title, channelId);

        protected ColumnBuilderConfiguration(ColumnIdentity identity) => this.Identity = identity ?? throw new ArgumentNullException(nameof(identity));

        /// <summary>
        /// Creates a column builder configuration that uses the specified title and column id for
        /// the column when calling TryBuild.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="channelId"></param>
        /// <returns></returns>
        public static ColumnBuilderConfiguration Create(string title, string channelId) => new(title, channelId);

        /// <summary>
        /// Creates a column builder that reuses a column that was resolved before. This can be used if one input column
        /// is required by several calculated columns.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ColumnBuilderConfiguration Use(IDiscreteDataColumn source) => new UseColumnBuilderConfiguration(source);

        /// <summary>
        /// Constructor creating a column builder without title or channel id. You should use this only 
        /// if you plan to Select an existing column.
        /// </summary>
        public ColumnBuilderConfiguration(): this("?", "?") { }

        /// <summary>
        /// Creates a column builder configuration that selects a column by the specified matching criteria
        /// when materializing the column
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        public ColumnBuilderConfiguration Select(ColumnSpec spec) => SelectFirst(spec);

        /// <summary>
        /// Creates a column builder configuration that selects a column by the specified matching criteria
        /// when materializing the column. The first matching specification that yields a result wins.
        /// </summary>
        /// <param name="specs"></param>
        /// <returns></returns>
        public ColumnBuilderConfiguration SelectFirst(params ColumnSpec[] specs) => new SelectionBuilderConfiguration(Identity, specs);
        
        /// <summary>
        /// Appends a unit conversion to the current builder configuration
        /// </summary>
        /// <param name="targetUnit"></param>
        /// <returns></returns>
        public ColumnBuilderConfiguration ConvertUnit(string targetUnit) => new UnitConversionBuilderConfiguration(this, targetUnit);

        /// <summary>
        /// Changes the unit of a column without doing any calculation. You can use this transformation if you do the calculation
        /// in a seperate step, for example if you convert mass/time to volume/time by doing a linear transformation using the
        /// liquid's density.
        /// </summary>
        /// <param name="targetUnit"></param>
        /// <returns></returns>
        public ColumnBuilderConfiguration AssignUnit(string targetUnit) => new UnitReassignmentBuilderConfiguration(this, targetUnit);


        /// <summary>
        /// Appends a multiplication by a constant factor to the current builder configuration
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public ColumnBuilderConfiguration MultiplyBy(double other) => TransformLinear(other, 0);

        /// <summary>
        /// Appends a multiplication by another column to the current builder configuration
        /// </summary>
        /// <param name="otherBuilderConfiguration"></param>
        /// <returns></returns>
        public ColumnBuilderConfiguration MultiplyBy(ColumnBuilderConfiguration otherBuilderConfiguration) => new FunctionBuilderConfiguration(Identity, this, otherBuilderConfiguration, (x, y) => x * y, "{0} * {1}");
        
        public ColumnBuilderConfiguration Add(ColumnBuilderConfiguration otherBuilderConfiguration) => new FunctionBuilderConfiguration(Identity, this, otherBuilderConfiguration, (x, y) => x + y, "{0} + {1}");
        public ColumnBuilderConfiguration Add(double other) => TransformLinear(1, other);
        public ColumnBuilderConfiguration Subtract(ColumnBuilderConfiguration otherBuilderConfiguration) => new FunctionBuilderConfiguration(Identity, this, otherBuilderConfiguration, (x, y) => x - y, "'{0}' - '{1}'");
        public ColumnBuilderConfiguration Subtract(double other) => TransformLinear(1, -other);
        public ColumnBuilderConfiguration IntegrateByTime() => new IntegrateByTimeConfigurationBuilder(Identity, this);

        /// <summary>
        /// Linear transformations provide an option to apply a linear function to a column. This should be used only if
        /// there is no better option, for example multiplying NOx sensor readings with 10^-6 because they are "ppm", and
        /// ppm is not a recognised unit.
        /// </summary>
        /// <param name="factor"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public ColumnBuilderConfiguration TransformLinear(double factor, double offset) => new LinearConstTransformationBuilderConfiguration(Identity, this, factor, offset);


        /// <summary>
        /// Materializes the column defined by the current column builder configuration
        /// </summary>
        /// <param name="sourceColumns"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public virtual ColumnBuilderBuildingResult TryBuild(IList<IDiscreteDataColumn> sourceColumns, [NotNullWhen(true)] out IDiscreteDataColumn? result)
        {
            result = null;
            return ColumnBuilderBuildingResult.CreateError(new ColumnBuilderBuildFailure(this, "Empty configuration defined."));
        }

        public IColumnBuilderForCalculation Calculate => new ForCalculation(this, Identity);

        protected ColumnIdentity Identity { get; private set; }



        // This must not be a ColumnBuilderConfiguration, otherwise we could accidently call "TryResolve".
        private class ChainElement //: ColumnBuilderConfiguration
        {
            protected ChainElement(ColumnBuilderConfiguration parentBuilderConfiguration, ColumnIdentity identity) =>  (Identity, ParentBuilderConfiguration) = (
                identity ?? throw new ArgumentNullException(nameof(identity)),
                parentBuilderConfiguration ?? throw new ArgumentNullException(nameof(parentBuilderConfiguration)));

            protected ColumnBuilderConfiguration ParentBuilderConfiguration { get; private set; }
            protected ColumnIdentity Identity { get; private set; }
        }
    }

    public interface IColumnBuilderForCalculation
    {
        IIntegralByTimeContainer IntegralByTime { get; }
        IRunningChangeContainer RunningChange { get; }
        public ColumnBuilderConfiguration DifferenceToFirstRow { get; }
    }

    public interface IIntegralByTimeContainer
    {
        ColumnBuilderConfiguration Over(ColumnBuilderConfiguration sourceColumnSelector);
    }

    public interface IRunningChangeContainer
    {
        ColumnBuilderConfiguration Over(TimeSpan duration);
    }
}
