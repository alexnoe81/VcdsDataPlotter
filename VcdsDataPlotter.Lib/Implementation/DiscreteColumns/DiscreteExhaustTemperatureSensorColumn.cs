using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.Interface;

namespace VcdsDataPlotter.Lib.Implementation
{
    public class DiscreteExhaustTemperatureSensorColumn : SingleValueColumnBase
    {
        public DiscreteExhaustTemperatureSensorColumn(IRawDataColumn rawData, int sensorIndex) : base(rawData)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(sensorIndex);
            this.sensorIndex = sensorIndex;
        }

        private int sensorIndex;

        public override IEnumerable<SingleDataItem> EnumerateDataItems()
        {
            foreach (var rawItem in this.RawData.EnumerateDataItems())
            {
                var capturedVarName = "Temp" + sensorIndex;
                var temperatureMatch = TemperatureParser.Match(rawItem.RawData);
                if (temperatureMatch.Success)
                {
                    double.TryParse(temperatureMatch.Groups[capturedVarName].ToString(), out var temperatureReading);
                    yield return new SingleDataItem(rawItem.TimeStamp, temperatureReading);
                }
            }
        }

        public override string ChannelId => base.ChannelId + "/" + sensorIndex.ToString();
        public override string Title => base.Title + "/" + sensorIndex.ToString();

        internal static Regex TemperatureParser = new Regex(@"S1/S2/S3/S4:\s*/\s*(?<Temp1>[0-9\+\-]+)/\s*(?<Temp2>[0-9\+\-]+)/\s*(?<Temp3>[0-9\+\-]+)/\s*(?<Temp4>[0-9\+\-]+).*");
    }

}
