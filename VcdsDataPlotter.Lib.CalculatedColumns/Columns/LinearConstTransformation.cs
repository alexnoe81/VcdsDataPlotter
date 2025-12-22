using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.CalculatedColumns.Columns;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.CalculatedColumns.Columns
{
    public class LinearConstTransformation : CalculatedColumnBase, IDiscreteDataColumn
    {
        protected LinearConstTransformation(IDiscreteDataColumn sourceColumn)
        {
            SourceColumn = sourceColumn ?? throw new ArgumentNullException(nameof(sourceColumn));
            Title = sourceColumn.Title;
            ChannelId = sourceColumn.ChannelId;
        }

        protected LinearConstTransformation(string title, string channelId, IDiscreteDataColumn sourceColumn, double factor, double offset) 
        {
            Title = title;
            ChannelId = channelId;
            SourceColumn = sourceColumn ?? throw new ArgumentNullException(nameof(sourceColumn));
            Factor = factor;
            Offset = offset;
        }

        public static LinearConstTransformation Create(string title, string channelId, IDiscreteDataColumn sourceColumn, double factor, double offset)
        {
            _ = sourceColumn ?? throw new ArgumentNullException(nameof(sourceColumn));

            LinearConstTransformation result = new LinearConstTransformation(
                title: title,
                channelId: channelId,
                sourceColumn: sourceColumn,
                factor: factor,
                offset: offset);

            return result;
        }

        public override string ToString()
        {
            return $"{Factor} * [{SourceColumn.ToString()}] + {Offset}";
        }

        public string? ChannelId { get; private set; }

        public string? Title { get; private set; }

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
