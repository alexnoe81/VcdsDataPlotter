using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.CalculatedColumns.ColumnSelection;

namespace VcdsDataPlotter.Lib.CalculatedColumns.ColumnSpec
{
    public static class KnownInputColumnSpecs
    {
        // TODO: Add localized texts? Put this into a configuration file?
        public static ColumnSelector VehicleSpeed = ColumnSelectors.SelectFirst(
            InputColumnSpec.ChannelIdIs("IDE00075"),
            InputColumnSpec.TitleContains("Vehicle speed"));
    }
}
