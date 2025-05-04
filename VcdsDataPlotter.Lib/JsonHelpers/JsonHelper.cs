using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.RawTable.Columnizer;

namespace VcdsDataPlotter.Lib.JsonHelpers
{
   /* public class JsonHelper
    {
        public static RawColumn2ValueColumnMap LoadMap(string filePath)
        {
            ArgumentNullException.ThrowIfNull(filePath);

            var serializer = CreateMappingSerializer();
            using var fileStream = File.OpenRead(filePath);
            using var reader = new StreamReader(fileStream);

            var result = (RawColumn2ValueColumnMap)(serializer.Deserialize(reader, typeof(RawColumn2ValueColumnMap)) ?? throw new FormatException("Null is not a valid map."));
            return result;
        }

        public static JsonSerializer CreateMappingSerializer()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();

            JsonSerializer result = new JsonSerializer();
            result.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            result.SerializationBinder = new InternalChannelMapSerializationBinder();
            result.NullValueHandling = NullValueHandling.Ignore;
            result.TypeNameHandling = TypeNameHandling.Auto;
            result.StringEscapeHandling = StringEscapeHandling.Default;
            result.Formatting = Formatting.Indented;

            return result;
        }
    }

    internal class InternalChannelMapSerializationBinder : ISerializationBinder
    {
        public void BindToName(Type serializedType, out string? assemblyName, out string? typeName)
        {
            if (serializedType == typeof(RawColumn2ValueColumnMapItem))
            {
                assemblyName = null;
                typeName = ChannelDescriptionName;
                return;
            }

            if (serializedType == typeof(RawColumn2ValueColumnMapItemItem))
            {
                assemblyName = null;
                typeName = OutputChannelName;
                return;
            }

            assemblyName = null;
            typeName = null;
        }

        public Type BindToType(string? assemblyName, string typeName)
        {
            return typeName switch
            {
                ChannelDescriptionName => typeof(RawColumn2ValueColumnMapItem),
                OutputChannelName => typeof(RawColumn2ValueColumnMapItemItem),
                _ => Type.GetType($"{typeName},{assemblyName}") ?? throw new ArgumentException($"Type \"{typeName},{assemblyName}\" not found.")
            };  
        }

        private const string ChannelDescriptionName = "ChannelDescription";
        private const string OutputChannelName = "OutputChannel";
    }*/
}
