using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.RawTable.Columnizer;

public class DiscreteNoxSensorsColumn : SingleValueColumnBase
{
    public DiscreteNoxSensorsColumn(IRawDataColumn rawData, int sensorIndex) : base(rawData)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(sensorIndex);
        this.sensorIndex = sensorIndex;
    }

    private int sensorIndex;

    public override IEnumerable<SingleDataItem> EnumerateDataItems()
    {
        foreach (var rawItem in this.RawData.EnumerateDataItems())
        {

            var noxMatch = NOxParser.Match(rawItem.RawData);
            if (noxMatch.Success)
            {
                double.TryParse(noxMatch.Groups["NOx1ppm"].ToString(), out var nox1);
                double.TryParse(noxMatch.Groups["NOx2ppm"].ToString(), out var nox2);

                if (sensorIndex == 1 && nox1 < 65500)
                {
                    yield return new SingleDataItem(rawItem.TimeStamp, nox1);
                }

                if (sensorIndex == 2 && nox2 < 65500)
                {
                    yield return new SingleDataItem(rawItem.TimeStamp, nox2);
                }
            }
        }
    }

    public override string ChannelId => base.ChannelId + "/" + sensorIndex.ToString();
    public override string Title => base.Title + "/" + sensorIndex.ToString();


    internal static Regex NOxParser = new Regex(@"NOx11:((?<NOx1ppm>\d+)\s*ppm|---)\s*/\s*NOx21:((?<NOx2ppm>\d+)\s*ppm|---)");
}
