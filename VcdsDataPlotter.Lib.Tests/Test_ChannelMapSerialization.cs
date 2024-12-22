using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.Implementation;
using VcdsDataPlotter.Lib.JsonHelpers;

namespace VcdsDataPlotter.Lib.Tests
{
    [TestClass]
    public class Test_ChannelMapSerialization
    {
        [TestMethod]
        public void Test_CreaetDefaultChannelReaderMap()
        {
            var serializer = JsonHelper.CreateMappingSerializer();

            var sb = new StringBuilder();
            var writer = new StringWriter(sb);

            var map = ChannelMaps.CreateDefaultMap();

            serializer.Serialize(writer, map);
            string entireString = sb.ToString();

            File.WriteAllText("ChannelReaderMap.json", entireString);

            var loadedMap = JsonHelper.LoadMap("ChannelReaderMap.json");
        }
    }
}
