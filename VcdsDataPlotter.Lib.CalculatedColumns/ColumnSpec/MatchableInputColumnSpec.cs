using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.CalculatedColumns.ColumnSpec
{
    public abstract class MatchableInputColumnSpec : InputColumnSpec
    {
        protected MatchableInputColumnSpec() { }
    }
}
