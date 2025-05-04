using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace VcdsDataPlotter.Lib.Tests
{
    [TestClass]
    public class Test_ChannelMapSerialization
    {
        [TestMethod]
        public void Test_CreaetDefaultChannelReaderMap()
        {
            var map = ChannelMaps.CreateDefaultMap();
            var mapString = JsonSerializer.Serialize(map);
         
            File.WriteAllText("ChannelReaderMap.json", mapString);
        }
    }
}
