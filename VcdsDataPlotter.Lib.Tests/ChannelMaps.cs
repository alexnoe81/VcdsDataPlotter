using System;
using System.Linq;
using System.Text;
using VcdsDataPlotter.Lib.RawTable.Columnizer;

namespace VcdsDataPlotter.Lib.Tests
{
    internal class ChannelMaps
    {
        public static RawColumn2ValueColumnMap CreateDefaultMap()
        {
            RawColumn2ValueColumnMap map = new();
            map.Mappings =
            [
                new RawColumn2ValueColumnMapItem()
                {
                    ChannelId = "IDE04090", Output =
                    [
                        new RawColumn2ValueColumnMapItemItem() { TypeName = typeof(DiscreteExhaustTemperatureSensorColumn).FullName, Arguments = [1] },
                        new RawColumn2ValueColumnMapItemItem() { TypeName = typeof(DiscreteExhaustTemperatureSensorColumn).FullName, Arguments = [2] },
                        new RawColumn2ValueColumnMapItemItem() { TypeName = typeof(DiscreteExhaustTemperatureSensorColumn).FullName, Arguments = [3] },
                        new RawColumn2ValueColumnMapItemItem() { TypeName = typeof(DiscreteExhaustTemperatureSensorColumn).FullName, Arguments = [4] },
                    ]
                },

                new RawColumn2ValueColumnMapItem()
                {
                    ChannelId = "IDE04098", Output =
                    [
                        new RawColumn2ValueColumnMapItemItem() { TypeName = typeof(DiscreteNoxSensorsColumn).FullName, Arguments = [1] },
                        new RawColumn2ValueColumnMapItemItem() { TypeName = typeof(DiscreteNoxSensorsColumn).FullName, Arguments = [2] }
                    ]
                },

                new RawColumn2ValueColumnMapItem()
                {
                    ChannelId = "*", Output =
                    [
                        new RawColumn2ValueColumnMapItemItem() { TypeName = typeof(SimpleDiscreteNumericColumn).FullName, Arguments = new object[0] }
                    ]
                }
            ];

            return map;
        }
    }
}
