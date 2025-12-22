using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.CalculatedColumns.Interface;

/// <summary>
/// An interpolating column is able to interpolate a value for any time stamp
/// </summary>
public interface IInterpolatingColumn : IDiscreteDataColumn
{
    SingleDataItem GetValue(TimeSpan timestamp);
}
