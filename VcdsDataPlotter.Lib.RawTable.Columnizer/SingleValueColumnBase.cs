using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.RawTable.Columnizer;

/// <summary>
/// A single value column provides atomic data, such as one temperature sensor from a column
/// that contains a string of four temperature sensors.
/// </summary>
public abstract class SingleValueColumnBase : IDiscreteDataColumn
{
    public SingleValueColumnBase(IRawDataColumn rawData)
    {
        this.RawData = rawData ?? throw new ArgumentNullException(nameof(rawData));
    }

    public abstract IEnumerable<SingleDataItem> EnumerateDataItems();

    protected IRawDataColumn RawData { get; private set; }

    public virtual string? ChannelId => RawData.ChannelId;
    public virtual string? Title => RawData.Title;
    public virtual string? Unit => RawData.RawUnit;


    protected static CultureInfo DefaultCulture = CultureInfo.InvariantCulture;
}
