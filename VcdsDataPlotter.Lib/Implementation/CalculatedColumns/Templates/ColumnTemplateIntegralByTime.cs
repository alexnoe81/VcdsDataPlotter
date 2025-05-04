using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.Implementation.CalculatedColumns.Templates;

public class ColumnTemplateIntegralByTime : ColumnTemplateBaseWithOneInputColumn
{
    public ColumnTemplateIntegralByTime()
    {
        TemplateName = "Integral by time";
    }

    /// <summary>
    /// Tests if an input column can be accepted. An IntegralByTimeColumn column needs something that is
    /// measured by time, such as speed (=distance per time) or mass flow.
    /// </summary>
    /// <param name="possibleInput"></param>
    /// <returns></returns>
    public override bool CanAcceptInput(IDiscreteDataColumn possibleInput)
    {
        _ = possibleInput ?? throw new ArgumentNullException(nameof(possibleInput));

        // This type of column can accept columns that provide data by time
        // if (_puh.CanBeIntegratedByTime(possibleInput.Unit))
            // return true;

        return false;
    }

    //private static IPhysicalUnitHelper _puh = new PhysicalUnitHelper();
}
