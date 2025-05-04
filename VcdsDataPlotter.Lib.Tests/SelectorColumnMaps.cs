using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.Implementation.StaticModel.SelectorColumns;

namespace VcdsDataPlotter.Lib.Tests
{
    internal class SelectorColumnMaps
    {
        public SelectorColumnMaps() { }

        public static SelectorColumnMap CreateDefaultMap()
        {
            SelectorColumnMap map = new();
            map.SelectorColumns =
            [
                new SelectorColumnStaticModel()
                {
                     ChannelId = "VIRT_NOX1",
                     Description = "NOx Sensor 1",
                     SourceChannels = [ "IDE04098/1", "IDE03140" ]
                },

                new SelectorColumnStaticModel()
                {
                     ChannelId = "VIRT_TOTAL_ADBLUE_1",
                     Description = "Total AdBlue consumption",
                     SourceChannels = ["IDE16115", "IDE03144"]
                },

                // TODO: Add special implementation using current dosing IDE11219
                new SelectorColumnStaticModel()
                {
                     ChannelId = "VIRT_TOTAL_ADBLUE_2",
                     Description = "Total AdBlue consumption",
                     SourceChannels = ["IDE16115", "IDE03144", "IDE11219"]
                }

            ];

            return map;
        }
    }
}
