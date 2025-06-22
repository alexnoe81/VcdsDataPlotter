using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Gui.ViewModel;
using VcdsDataPlotter.Lib.RawTable.Columnizer;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;
using VcdsDataPlotter.Lib.RawTableReader;
using VcdsDataPlotter.Lib.RawTableReader.Interface;
using VcdsDataPlotter.Lib.CalculatedColumns;

namespace VcdsDataPlotter.Gui.Model
{
    public class Document
    {
        private Document() { }

        public static Document LoadFile(string filePath)
        {
            Document result = new Document();
            result.objectLogger = ClassLogger.ForContext(nameof(filePath), filePath);

            // Currently, we assume that we have CSV files
            using var fileStream = File.OpenRead(filePath);
            using var fileReader = new StreamReader(fileStream, Encoding.Latin1, true);
            IRawTable table = CsvTable.Open(fileReader, ",");
            VcdsTableColumnizer vcdsFile = VcdsTableColumnizer.Open(table);
            result.source = vcdsFile;

            FileInfo fi = new FileInfo(filePath);
            result.RawColumns = (IRawDataColumn[])vcdsFile.RawDataColumns.Clone();
            result.DiscreteColumns = (IDiscreteDataColumn[])vcdsFile.DiscreteDataColumns.Clone();

            result.FileTime = fi.LastWriteTime;
            result.RecordingTimestamp = vcdsFile.RecordingTimestamp;

            if (KnownCalculatedColumnsFactory.TryCreateTraveledDistanceColumn(result.DiscreteColumns, out var distanceColumn))
            {
                result.CalculatedColumns = new IDiscreteDataColumn[] { distanceColumn };
            }

            return result;
        }

        public IDiscreteDataColumn[] DiscreteColumns { get; set; }
        public IRawDataColumn[] RawColumns { get; set; }

        public IDiscreteDataColumn[] CalculatedColumns { get; set; }

        public DateTime FileTime { get; private set; }
        public DateTime RecordingTimestamp { get; private set; }


        private static ILogger ClassLogger = Serilog.Log.Logger.ForClass(typeof(Document));
        private ILogger? objectLogger;
        private VcdsTableColumnizer? source;
    }
}
