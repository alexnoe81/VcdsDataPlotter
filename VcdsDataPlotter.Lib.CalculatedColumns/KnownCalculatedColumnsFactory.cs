using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.CalculatedColumns.ColumnSpec;
using VcdsDataPlotter.Lib.CalculatedColumns.Math;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.CalculatedColumns
{
    public static class KnownCalculatedColumnsFactory
    {
        public static bool TryCreateTraveledDistanceColumn(IEnumerable<IDiscreteDataColumn> allAvailableSourceColumns, [NotNullWhen(true)] out IDiscreteDataColumn? result)
        {
            _ = allAvailableSourceColumns ?? throw new ArgumentNullException(nameof(allAvailableSourceColumns));

            if (KnownInputColumnSpecs.VehicleSpeed.TryResolve(allAvailableSourceColumns, out var speedColumn))
            {
                result = UnitTransformation.Create(IntegralByTimeColumn.Create("Traveled distance", "VIRT_DISTANCE", speedColumn), "m");
                return true;
            }

            result = null;
            return false;
        }
    }
}
