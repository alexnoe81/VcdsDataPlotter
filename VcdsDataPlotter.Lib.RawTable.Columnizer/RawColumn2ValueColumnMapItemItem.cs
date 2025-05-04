using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.RawTable.Columnizer;

public class RawColumn2ValueColumnMapItemItem
{
    public RawColumn2ValueColumnMapItemItem()
    {

    }

    public IDiscreteDataColumn CreateColumn(IRawDataColumn rawDataColumn)
    {
        if (TypeName is null)
            throw new InvalidOperationException("No type name was defined.");

        var type = Type.GetType(TypeName);
        if (type is null)
            throw new InvalidOperationException($"Unknown type specified: \"{TypeName}\"");

        object[] arguments = new object[] { rawDataColumn }.Concat(Arguments ?? Array.Empty<object>()).ToArray();
        var result = Activator.CreateInstance(type, arguments);
        return (IDiscreteDataColumn)result;
    }

    public string? TypeName { get; set; }
    public object[]? Arguments { get; set; }
}
