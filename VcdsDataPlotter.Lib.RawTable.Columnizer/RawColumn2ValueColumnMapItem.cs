using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.RawTable.Columnizer;

/// <summary>
/// One Item handles one measurement channel id
/// </summary>
public class RawColumn2ValueColumnMapItem
{
    public RawColumn2ValueColumnMapItem()
    {
    }

    public IEnumerable<IDiscreteDataColumn> CreateColumns(IRawDataColumn rawDataColumn)
    {
        ArgumentNullException.ThrowIfNull(rawDataColumn);
        if (Output is null)
            throw new InvalidOperationException("No mappings were defined.");

        foreach (var item in Output)
            yield return item.CreateColumn(rawDataColumn);
    }

    public string? ChannelId { get; set; }

    public RawColumn2ValueColumnMapItemItem[]? Output { get; set; }
}
