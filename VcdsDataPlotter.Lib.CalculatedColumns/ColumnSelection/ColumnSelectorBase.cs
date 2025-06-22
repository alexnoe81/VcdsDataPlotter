using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.CalculatedColumns.ColumnSpec;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.CalculatedColumns.ColumnSelection
{
    public abstract class ColumnSelector
    {
        protected ColumnSelector() { }
        public abstract bool TryResolve(IEnumerable<IDiscreteDataColumn> columns, out IDiscreteDataColumn result);
    }
}
