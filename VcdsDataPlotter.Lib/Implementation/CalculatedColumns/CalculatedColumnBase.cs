using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.Implementation.CalculatedColumns
{
    internal abstract class CalculatedColumnWithOneInputColumn
    {
        public CalculatedColumnWithOneInputColumn() { }

        public IDiscreteDataColumn Input { get; set; }
    }


    /// <summary>
    /// A calculated column argument needs a value and a unit. We could require the user to
    /// use one specific unit, like distances in meters or times in seconds.
    /// </summary>
    public class CalculatedColumnArgumentTemplate
    {
        public CalculatedColumnArgumentTemplate() { }

    }    
}
