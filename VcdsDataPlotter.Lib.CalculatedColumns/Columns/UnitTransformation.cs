using System;
using VcdsDataPlotter.Lib.Physics;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.CalculatedColumns.Columns
{
    public class UnitTransformation : LinearConstTransformation
    {
        private UnitTransformation(IDiscreteDataColumn sourceColumn, string targetUnit) 
            :base(sourceColumn) 
        {
            _ = sourceColumn ?? throw new ArgumentNullException(nameof(sourceColumn));

            Factor = UnitHelpers.GetConversionFactor(sourceColumn.Unit, targetUnit);
            Offset = 0.0;

            this.targetUnit = targetUnit;
        }

        public static UnitTransformation Create(IDiscreteDataColumn sourceColumn, string targetUnit)
        {
            _ = sourceColumn ?? throw new ArgumentNullException(nameof(sourceColumn));
            _ = targetUnit ?? throw new ArgumentNullException(nameof(targetUnit));

            if (!UnitHelpers.IsConvertible(sourceColumn.Unit, targetUnit))
                throw new ArgumentException($"Cannot convert '  '{sourceColumn.Unit}' to '{targetUnit}'");

            return new(sourceColumn, targetUnit);
        }

        public static bool CanCreate(IDiscreteDataColumn sourceColumn, string targetUnit) => UnitHelpers.IsConvertible(sourceColumn.Unit, targetUnit);

        public override string? Unit => targetUnit;
        private string targetUnit;
    }

    public class UnitReassignment : LinearConstTransformation
    {
        private UnitReassignment(IDiscreteDataColumn sourceColumn, string targetUnit)
            : base(sourceColumn)
        {
            Factor = 1.0;
            Offset = 0.0;
            this.targetUnit = targetUnit;
        }

        public static UnitReassignment Create(IDiscreteDataColumn sourceColumn, string targetUnit)
        {
            _ = sourceColumn ?? throw new ArgumentNullException(nameof(sourceColumn));
            _ = targetUnit ?? throw new ArgumentNullException(nameof(targetUnit));

            // We do not test convertability here because it is not a conversion

            return new(sourceColumn, targetUnit);
        }

        public static bool CanCreate(IDiscreteDataColumn sourceColumn, string targetUnit) => UnitHelpers.IsConvertible(sourceColumn.Unit, targetUnit);

        public override string? Unit => targetUnit;
        private string targetUnit;
    }
}
