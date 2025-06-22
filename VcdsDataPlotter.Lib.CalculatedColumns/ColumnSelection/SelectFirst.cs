using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.CalculatedColumns.ColumnSpec;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.CalculatedColumns.ColumnSelection
{
    public class SelectFirst : ColumnSelector
    {
        public SelectFirst(params MatchableInputColumnSpec[] criteria)
        {
            this.criteria = new ReadOnlyCollection<MatchableInputColumnSpec>([.. criteria]);
        }

        public override bool TryResolve(IEnumerable<IDiscreteDataColumn> columns, out IDiscreteDataColumn result)
        {
            foreach (var criterion in this.criteria)
            {
                foreach (var column in columns)
                {
                    if (criterion.Matches(column))
                    {
                        result = column;
                        return true;
                    }
                }
            }

            result = null;
            return false;
        }

        private ReadOnlyCollection<MatchableInputColumnSpec> criteria;
    }

}
