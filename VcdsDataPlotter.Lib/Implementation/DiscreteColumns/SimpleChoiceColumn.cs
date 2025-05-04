using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.Implementation.DiscreteColumns
{
    public class SimpleChoiceColumn : IDiscreteDataColumn
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
