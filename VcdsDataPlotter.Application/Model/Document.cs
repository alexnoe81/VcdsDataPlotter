using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Gui.ViewModel;
using VcdsDataPlotter.Lib.Implementation;
using VcdsDataPlotter.Lib.Implementation.RawData;
using VcdsDataPlotter.Lib.Interface;

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
            VcdsRecordedFile vcdsFile = VcdsRecordedFile.Open(table);
            result.source = vcdsFile;

            FileInfo fi = new FileInfo(filePath);
            result.DiscreteColumns = (IDiscreteDataColumn[])vcdsFile.DiscreteDataColumns.Clone();
            result.RawColumns = (IRawDataColumn[])vcdsFile.RawDataColumns.Clone();
            result.FileTime = fi.LastWriteTime;
            result.RecordingTimestamp = vcdsFile.RecordingTimestamp;

            return result;
        }

        public IDiscreteDataColumn[] DiscreteColumns { get; set; }
        public IRawDataColumn[] RawColumns { get; set; }

        public DateTime FileTime { get; private set; }
        public DateTime RecordingTimestamp { get; private set; }


        private static ILogger ClassLogger = Serilog.Log.Logger.ForClass(typeof(Document));
        private ILogger? objectLogger;
        private VcdsRecordedFile? source;
    }
}
