namespace VcdsDataPlotter.Lib.Interface
{
    /// <summary>
    /// Reads a table that consists of single rows and single columns. This could be a CSV file, an ODS document etc.
    /// </summary>
    public interface IRawTable
    {
        int NumberOfRows { get; }
        int NumberOfColumns { get; }
        public string? GetCellContent(int row, int column);
    }
    
    /// <summary>
    /// A VcdsLogReader reads a log file from VCDS, meaning that each semantically meaningful column
    /// consists of one column for time stamp and one column for the sensor value(s)
    /// </summary>
    public interface IVcdsLogReader
    {

    }
}
