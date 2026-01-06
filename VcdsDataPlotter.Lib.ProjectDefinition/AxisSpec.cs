using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VcdsDataPlotter.Lib.ProjectDefinition;

/// <summary>
/// Defines one axis
/// </summary>
public class AxisSpec
{
    public string? Title { get; set; }

    [XmlIgnore]
    public AxisEndpoint? MinValue { get; set; }

    [XmlIgnore]
    public AxisEndpoint? MaxValue { get; set; }

    [XmlElement("Range")]
    public string RangeString
    {
        get => $"[{SerializeAxisEndpoint(MinValue)} .. {SerializeAxisEndpoint(MaxValue)}]";
        set
        {
            if (value is null)
            {
                (MinValue, MaxValue) = (null, null);
                return;
            }

            var pieces = value.Trim('[', ']').Split("..");
            if (pieces.Length != 2)
                throw new FormatException("Bad format for range. Input was: " + value);

            MinValue = DeserializeAxisEndpoint(pieces[0]);
            MaxValue = DeserializeAxisEndpoint(pieces[1]);
        }
    }


    private string SerializeAxisEndpoint(AxisEndpoint? value)
    {
        if (value is null)
            return "null";

        if (value.IsAuto)
            return "auto";

        return value.Value.Value.ToString(enUS);
    }

    private AxisEndpoint? DeserializeAxisEndpoint(string valueText)
    {
        valueText = valueText.Trim();
        if (valueText is null)
            return null;

        var valueTextLower = valueText.ToLowerInvariant();
        if (valueTextLower == "auto")
            return AxisEndpoint.Auto;
        if (valueTextLower == "null")
            return null;

        return new AxisEndpoint(double.Parse(valueText, NumberStyles.Number, enUS));
    }


    [XmlIgnore]
    public double[]? TickIntervals { get; set; }

    [XmlElement("TickIntervals")]
    public string TickIntervalsString
    {
        get => string.Join(";", (TickIntervals ?? Array.Empty<double>()).Select(x => x.ToString(enUS)));
        set
        {
            if (value is null)
            {
                TickIntervals = Array.Empty<double>();
            }
            else
            {
                string[] pieces = value.Split(";");
                TickIntervals = new double[pieces.Length];
                for (int j = 0; j < pieces.Length; j++)
                    TickIntervals[j] = double.Parse(pieces[j]);
            }
        }
    }

    private static CultureInfo enUS = CultureInfo.GetCultureInfo("en-US");
}
