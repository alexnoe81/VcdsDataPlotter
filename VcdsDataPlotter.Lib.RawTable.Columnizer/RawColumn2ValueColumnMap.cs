using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VcdsDataPlotter.Lib.RawTable.Columnizer;

/// <summary>
/// Represents a mapping that describes how to map raw columns to value providing columns. One raw column may expose
/// one or more readings. For example, a column may contain a string like this:
/// NOx11:147 ppm / NOx21:16 ppm
/// 
/// Currently, I hope that this is not vehicle dependent, otherwise the user needs a way to select a channel mapping.
/// </summary>
public class RawColumn2ValueColumnMap
{
    public RawColumn2ValueColumnMap()
    {
    }

    public bool TryGetMapping(string channelId, out RawColumn2ValueColumnMapItem? result)
    {
        _ = channelId ?? throw new ArgumentNullException(nameof(channelId));
        if (channelId.Length == 0) throw new ArgumentException("ChannelId must not be empty.", nameof(channelId));
        
        if (Mappings is null)
            throw new InvalidOperationException("No mappings were defined.");

        var channelIdUpper = channelId.ToUpperInvariant();
        result = Mappings.FirstOrDefault(item => item.ChannelId?.ToUpperInvariant() == channelIdUpper) ?? Mappings.FirstOrDefault(item => item.ChannelId == "*");
        return result is not null;
    }

    public RawColumn2ValueColumnMapItem[]? Mappings { get; set; }
}
