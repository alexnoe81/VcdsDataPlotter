using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.Interface;

namespace VcdsDataPlotter.Lib.Implementation
{
    public class CsvTable : IRawTable
    {
        private CsvTable(TextReader reader) : this(reader, ",")
        {
        }

        private CsvTable(TextReader reader, string separator)
        {
            this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
            this.separator = separator ?? throw new ArgumentNullException(nameof(separator));
        }

        public static CsvTable Open(TextReader reader, string separator)
        {
            var result = new CsvTable(reader, separator);
            result.Initialize();
            return result;
        }

        private void Initialize()
        {
            var temp = new List<string[]>();
            numberOfColumns = 0;
            while (this.reader.ReadLine() is string nextLine)
            {
                var cells = nextLine.Split(this.separator, StringSplitOptions.TrimEntries);
                if (cells is string[] { Length: > 0 })
                {
                    temp.Add(cells);

                    // Normally, all rows should have the same number of columns
                    numberOfColumns = Math.Max(numberOfColumns, cells.Length);
                }
            }

            this.data = temp;
        }

        public string? GetCellContent(int row, int column)
        {
            CheckInitialized();

            ArgumentOutOfRangeException.ThrowIfNegative(row);
            ArgumentOutOfRangeException.ThrowIfNegative(column);

            if (row >= NumberOfRows)
                return null;
            if (column >= data![row].Length)
                return null;

            return data[row][column];
        }

        public int NumberOfRows
        {
            get
            {
                CheckInitialized();
                return data!.Count;
            }
        }

        public int NumberOfColumns
        {
            get
            {
                CheckInitialized();
                return numberOfColumns;
            }
        }

        private void CheckInitialized()
        {
            if (data is null)
                throw new InvalidOperationException("Object is not initialized.");
        }

        private TextReader reader;
        private string separator;
        private List<string[]>? data;
        private int numberOfColumns;
    }
}
