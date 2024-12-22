using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.Interface;

namespace VcdsDataPlotter.Lib.Implementation
{
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
            ArgumentException.ThrowIfNullOrWhiteSpace(channelId);
            if (Mappings is null)
                throw new InvalidOperationException("No mappings were defined.");

            var channelIdUpper = channelId.ToUpperInvariant();
            result = Mappings.FirstOrDefault(item => item.ChannelId?.ToUpperInvariant() == channelIdUpper) ?? Mappings.FirstOrDefault(item => item.ChannelId == "*");
            return result is not null;
        }

        public RawColumn2ValueColumnMapItem[]? Mappings { get; set; }
    }

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

    public class RawColumn2ValueColumnMapItemItem
    {
        public RawColumn2ValueColumnMapItemItem()
        {

        }

        public IDiscreteDataColumn CreateColumn(IRawDataColumn rawDataColumn)
        {
            if (TypeName is null)
                throw new InvalidOperationException("No type name was defined.");

            var type = Type.GetType(TypeName);
            if (type is null)
                throw new InvalidOperationException($"Unknown type specified: \"{TypeName}\"");

            object[] arguments = new object[] { rawDataColumn }.Concat(Arguments ?? Array.Empty<object>()).ToArray();
            var result = Activator.CreateInstance(type, arguments);
            return (IDiscreteDataColumn)result;
        }

        public string? TypeName { get; set; }
        public object[]? Arguments { get; set; }
    }
}
