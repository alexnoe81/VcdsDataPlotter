using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.Implementation.DiscreteColumns
{
    /// <summary>
    /// A SelectorColumn is a column that selects its input from several possible source columns
    /// where only one needs to be present. For example, NOx sensor readings might be selected from
    /// a column containing combined NOx1/2 readings, or from columns with single readings
    /// </summary>
    internal class SelectorColumn : IDiscreteDataColumn
    {

        public string? ChannelId => throw new NotImplementedException();

        public string? Title => throw new NotImplementedException();

        public string? Unit => throw new NotImplementedException();

        public IEnumerable<SingleDataItem> EnumerateDataItems()
        {
            throw new NotImplementedException();
        }
    }
}
