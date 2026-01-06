using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VcdsDataPlotter.Lib.CalculatedColumns.ColumnsBuilders
{
    public class ColumnBuilderBuildFailure
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">The ColumnBuilderConfiguration that encountered the problem</param>
        /// <param name="error">Text description of the problem</param>
        /// <exception cref="ArgumentNullException">if <paramref name="source"/> is null</exception>
        public ColumnBuilderBuildFailure(ColumnBuilderConfiguration source, string error)
        {
            Error = error;
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Reasons = new ReadOnlyCollection<ColumnBuilderBuildFailure>(Array.Empty<ColumnBuilderBuildFailure>());
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="source">The ColumnBuilderConfiguration that encountered the problem</param>
        /// <param name="error">Text description of the problem</param>
        /// <param name="reasons">If <paramref name="source"/> relied on other column builders to
        /// build a source column, this parameter contains the results of those source column builders</param>
        /// <exception cref="ArgumentNullException">if <paramref name="source"/> is null</exception>
        public ColumnBuilderBuildFailure(ColumnBuilderConfiguration source, string error, IList<ColumnBuilderBuildFailure> reasons)
        {
            Error = error;
            Source = source ?? throw new ArgumentNullException(nameof(source)); ;
            Reasons = new ReadOnlyCollection<ColumnBuilderBuildFailure>(reasons);
        }

        public override string ToString() => $"{Source}: {Error} [ {Reasons.Count} reason(s) ]";

        /// <summary>
        /// Returns a text description of the problem
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// Returns the ColumnBuilderConfiguration that observed the problem and generated the error
        /// </summary>
        public ColumnBuilderConfiguration Source { get; private set; }

        /// <summary>
        /// If a ColumnBuilderConfiguration tried to build a calculated column from other source columns, 
        /// and failed to do so, this property contains the result objects from these source column builders.
        /// </summary>
        public IList<ColumnBuilderBuildFailure> Reasons { get; private set; }
    }
}
