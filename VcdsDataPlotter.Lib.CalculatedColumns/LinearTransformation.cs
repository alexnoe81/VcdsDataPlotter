using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.CalculatedColumns
{
    public class LinearTransformation : CalculatedColumnBase, IDiscreteDataColumn
    {
        public LinearTransformation(IDiscreteDataColumn sourceColumn) 
        {
            this.SourceColumn = sourceColumn ?? throw new ArgumentNullException(nameof(sourceColumn));
        }

        public override string ToString()
        {
            return $"{Factor} * [{SourceColumn.ToString()}] + {Offset}";
        }

        public string? ChannelId => SourceColumn.ChannelId;

        public string? Title => SourceColumn.Title;

        public virtual string? Unit => SourceColumn.Unit;

        public IEnumerable<SingleDataItem> EnumerateDataItems()
        {
            foreach (var item in SourceColumn.EnumerateDataItems())
            {
                yield return new(item.TimeStamp, Factor * item.RawData + Offset);
            }
        }

        public double Factor { get; set; } = 1.0;
        public double Offset { get; set; } = 0.0;

        protected IDiscreteDataColumn SourceColumn { get; private set; }
    }
}
