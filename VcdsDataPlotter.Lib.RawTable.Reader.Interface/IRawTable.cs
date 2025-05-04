using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VcdsDataPlotter.Lib.RawTableReader.Interface;

/// <summary>
/// Reads a table that consists of single rows and single columns. This could be a CSV file, an ODS document etc.
/// </summary>
public interface IRawTable
{
    int NumberOfRows { get; }
    int NumberOfColumns { get; }
    public string? GetCellContent(int row, int column);
}
