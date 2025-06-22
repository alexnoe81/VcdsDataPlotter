using System;
using VcdsDataPlotter.Lib.Physics;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.CalculatedColumns.Math
{
    public class UnitTransformation : LinearTransformation
    {
        private UnitTransformation(IDiscreteDataColumn sourceColumn, string targetUnit) 
            :base(sourceColumn) 
        {
            Factor = UnitHelpers.GetConversionFactor(sourceColumn.Unit, targetUnit);
            this.targetUnit = targetUnit;
        }

        public static UnitTransformation Create(IDiscreteDataColumn sourceColumn, string targetUnit)
        {
            if (!UnitHelpers.IsConvertible(sourceColumn.Unit, targetUnit))
                throw new ArgumentException($"Cannot convert '  '{sourceColumn.Unit}' to '{targetUnit}'");

            return new(sourceColumn, targetUnit);
        }

        public static bool CanCreate(IDiscreteDataColumn sourceColumn, string targetUnit) => UnitHelpers.IsConvertible(sourceColumn.Unit, targetUnit);

        public override string? Unit => targetUnit;
        private string targetUnit;
    }
}
