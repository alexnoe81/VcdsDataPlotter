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
        public ColumnBuilderBuildFailure(ColumnBuilderConfiguration source, string error)
        {
            this.Error = error;
            this.Source = source;
            this.Reasons = new ReadOnlyCollection<ColumnBuilderBuildFailure>(Array.Empty<ColumnBuilderBuildFailure>());
        }

        public ColumnBuilderBuildFailure(ColumnBuilderConfiguration source, string error, IList<ColumnBuilderBuildFailure> reasons)
        {
            this.Error = error;
            this.Source = source;
            this.Reasons = new ReadOnlyCollection<ColumnBuilderBuildFailure>(reasons);
        }

        public override string ToString() => $"{Source}: {Error} [ {Reasons.Count} reason(s) ]";

        public string Error { get; private set; }
        public ColumnBuilderConfiguration Source { get; private set; }

        public IList<ColumnBuilderBuildFailure> Reasons { get; private set; }
    }
}
