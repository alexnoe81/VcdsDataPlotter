namespace VcdsDataPlotter.Lib.Physics;

public class UnitHelpers
{
    public static bool IsConvertible(string unitFrom, string unitTo)
    {
        if (unitFrom == null || unitTo == null)
            return false;

        // TODO: Actually, the conversion factor from "ppm" to "" is 1/1.000.000
        if (unitFrom.Length == 0 || unitTo.Length == 0)
            return false;

        unitFrom = NormalizeUnit(unitFrom);
        unitTo = NormalizeUnit(unitTo);
        var first = DeconstructUnit(unitFrom);
        var second = DeconstructUnit(unitTo);

        if (GetBaseUnit(first.Nominator) != GetBaseUnit(second.Nominator)) return false;
        if (first.Denominator is not null ^ second.Denominator is not null) return false;
        if (first.Denominator is not null)
        {
            if (GetBaseUnit(first.Denominator) != GetBaseUnit(second.Denominator!))
                return false;
        }

        return true;
    }

    public static double GetConversionFactor(string unitFrom, string unitTo)
    {
        return GetConversionFactor(unitFrom, unitTo, new());
    }

    private static double GetConversionFactor(string unitFrom, string unitTo, HashSet<(string, string)> stack)
    {
        unitFrom = NormalizeUnit(unitFrom);
        unitTo = NormalizeUnit(unitTo);
        var first = DeconstructUnit(unitFrom);
        var second = DeconstructUnit(unitTo);

        try
        {
            if (stack.Contains((unitFrom, unitTo)))
                throw new InvalidOperationException($"Internal error. Infinite recursion happened in {nameof(GetConversionFactor)}.");

            stack.Add((unitFrom, unitTo));

            if (first.Denominator is null ^ second.Denominator is null)
            {
                // If one unit has a denominator, but the other doesn't, they are incompatible
                throw new ArgumentException($"Units '{unitFrom}' and '{unitTo}' are incompatible.");
            }

            // Handle very simple cases
            if (unitTo == unitFrom) return 1.0;

            if (unitTo == "g" || unitTo == "l")
            {
                if (unitFrom.StartsWith("k"))
                    return 1_000;
                if (unitFrom.StartsWith("m"))
                    return 1.0 / 1_000;
                if (unitFrom.StartsWith("µ"))
                    return 1.0 / 1_000_000;
            }

            if (unitTo == "s")
            {
                if (unitFrom == "min")
                    return 60;
                if (unitFrom == "h")
                    return 3600;
            }

            if (unitFrom == "g" || unitFrom == "l" || unitFrom == "s")
            {
                return 1.0 / GetConversionFactor(unitTo, unitFrom, stack);
            }

            // Handle more complex cases

            var baseUnitFromNominator = GetBaseUnit(first.Nominator);
            var baseUnitToNominator = GetBaseUnit(second.Nominator);
            if (baseUnitFromNominator != baseUnitToNominator)
                throw new ArgumentException($"Units '{unitFrom}' and '{unitTo}' cannot be converted into each other.");

            var nominatorFactor1 = GetConversionFactor(first.Nominator, baseUnitFromNominator);
            var nominatorFactor2 = GetConversionFactor(second.Nominator, baseUnitToNominator);
            var nominatorFactor = nominatorFactor1 / nominatorFactor2;

            if (first.Denominator is { Length: > 0 } && second.Denominator is { Length: > 0 })
            {
                var denominatorFactor = GetConversionFactor(first.Denominator, second.Denominator);
                var result = nominatorFactor / denominatorFactor;
                return result;
            }
            else
            {
                return nominatorFactor;
            }
        }
        finally
        {
            stack.Remove((unitFrom, unitTo));
        }
    }

    private static string NormalizeUnit(string unit)
    {
        return unit.Replace(" ", "").ToLowerInvariant();
    }

    public static string GetBaseUnit(string unit)
    {
        unit = NormalizeUnit(unit);

        return unit switch
        {
            "kg" => "g",
            "g" => "g",
            "mg" => "g",
            "µg" => "g",
            "h" => "s",
            "min" => "s",
            "s" => "s",
            "l" => "l",
            "ml" => "l",
            _ => throw new ArgumentException($"Unit '{unit}' is not supported.", nameof(unit))
        };
    }

    public static (string Nominator, string? Denominator) DeconstructUnit(string entireUnit)
    {
        _ = entireUnit ?? throw new ArgumentNullException(nameof(entireUnit));

        // Note: using ToLowerInvariant like this only works because we don't need to use M (Mega) vs. m (milli) here.
        //       If we wanted to support M, we would need to exclude the m from being capitalized.
        var slashPos = entireUnit.IndexOf('/');
        if (slashPos == -1)
        {
            // There is no slash
            return (entireUnit.Trim().ToLowerInvariant(), null);
        }
        else
        {
            return (entireUnit.Substring(0, slashPos).Trim().ToLowerInvariant(), entireUnit.Substring(slashPos + 1).Trim().ToLowerInvariant());
        }
    }

}
