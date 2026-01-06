using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VcdsDataPlotter.Lib.ProjectDefinition;

/// <summary>
/// And AxisEndpoint defines the start or end value to use for an axis. It can either be set to auto,
/// or it can be a fixed value.
/// </summary>
/// <remarks>
/// For ShouldSerialize, see https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls-design/how-to-designer-properties-shouldserialize-reset
/// </remarks>
public class AxisEndpoint : IEquatable<AxisEndpoint>
{
    /// <summary>
    /// Should be only used by serializer
    /// </summary>
    public AxisEndpoint() { }
    public AxisEndpoint(double fixedValue) => Value = fixedValue;

    public static AxisEndpoint Auto { get; } = new AxisEndpoint() { IsAuto = true };

    public bool ShouldSerializeIsAuto() => IsAuto;

    public override int GetHashCode()
    {
        if (IsAuto)
            return true.GetHashCode();
        else
            return Value.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is not AxisEndpoint other)
            return false;

        return Equals(other);
    }

    public bool Equals(AxisEndpoint? other)
    {
        if (other is null)
            return false;

        if (other.IsAuto && IsAuto) return true;
        if (other.IsAuto != IsAuto) return false;

        if (other.Value == Value) return true;

        return false;
    }

    public bool ShouldSerializeValue => Value is not null;
    public bool IsAuto { get; set; }
    public double? Value { get; set; }
}
