using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.CalculatedColumns.Columns;
using VcdsDataPlotter.Lib.CalculatedColumns.Interface;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.CalculatedColumns.Columns
{
    public class MultiColumnCalculation : CalculatedColumnBase, IDiscreteDataColumn
    {
        protected MultiColumnCalculation(string title, string channelId, IDiscreteDataColumn inputColumn1, IDiscreteDataColumn inputColumn2, Func<double, double, double> @operator) 
        {
            Title = title;
            ChannelId = channelId;
            InputColumn1 = inputColumn1 ?? throw new ArgumentNullException(nameof(inputColumn1));
            InputColumn2 = inputColumn2 ?? throw new ArgumentNullException(nameof(inputColumn2));
            Operator = @operator ?? throw new ArgumentNullException(nameof(@operator));

            if (InputColumn2 is not IInterpolatingColumn)
                InputColumn2 = InterpolatingColumn.Create(InputColumn2);
        }     
        
        public static MultiColumnCalculation Create(string title, string channelId, IDiscreteDataColumn inputColumn1, IDiscreteDataColumn inputColumn2, Func<double, double, double> @operator)
        {
            _ = inputColumn1 ?? throw new ArgumentNullException(nameof(inputColumn1));
            _ = inputColumn2 ?? throw new ArgumentNullException(nameof(inputColumn2));
            _ = @operator ?? throw new ArgumentNullException(nameof(@operator));

            var result = new MultiColumnCalculation(
                title,
                channelId,
                inputColumn1,
                inputColumn2,
                @operator);
            
            return result;
        }


        // TODO: ??
        public override string ToString()
        {
            return $"";
        }

        public string? ChannelId { get; private set; }

        public string? Title { get; private set; }

        // TODO: If we multiply columns, we need to multiply the units, if we add them, the units must be compatible...?
        //       This implementation is a hack!
        public virtual string? Unit => String.IsNullOrWhiteSpace(InputColumn1.Unit) ? InputColumn2.Unit : InputColumn1.Unit;

        public virtual IEnumerable<SingleDataItem> EnumerateDataItems()
        {
            // TODO: Here we need to handle units better
            foreach (var item in InputColumn1.EnumerateDataItems())
            {
                var other = (InputColumn2 as IInterpolatingColumn)!.GetValue(item.TimeStamp);

                yield return new(item.TimeStamp, Operator(item.RawData, other.RawData));
            }
        }

        protected IDiscreteDataColumn InputColumn1 { get; private set; }
        protected IDiscreteDataColumn InputColumn2 { get; private set; }

        protected Func<double, double, double> Operator { get; private set; }
    }

    // Note: This does currently not take into account units, or even scaling
    public class ProductColumn: MultiColumnCalculation
    {
        protected ProductColumn(string title, string channelId, IDiscreteDataColumn inputColumn1, IDiscreteDataColumn inputColumn2)
            : base(title, channelId, inputColumn1, inputColumn2, (a, b) => a*b)
        {
        }
    }

    public class SumColumn : MultiColumnCalculation
    {
        protected SumColumn(string title, string channelId, IDiscreteDataColumn inputColumn1, IDiscreteDataColumn inputColumn2)
            : base(title, channelId, inputColumn1, inputColumn2, (a, b) => a + b)
        {
        }
    }
}
