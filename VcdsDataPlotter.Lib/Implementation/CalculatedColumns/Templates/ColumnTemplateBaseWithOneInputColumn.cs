using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.Implementation.CalculatedColumns.Templates;

public abstract class ColumnTemplateBaseWithOneInputColumn : ColumnTemplateBase
{
    protected ColumnTemplateBaseWithOneInputColumn()
    {
    }

    /// <summary>
    /// Many calculated columns require a specific kind of input. This function checks if a specific column
    /// can be used
    /// </summary>
    /// <param name="possibleInput"></param>
    /// <returns></returns>
    public abstract bool CanAcceptInput(IDiscreteDataColumn possibleInput);
}
