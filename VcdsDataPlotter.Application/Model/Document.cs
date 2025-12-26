using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Gui.ViewModel;
using VcdsDataPlotter.Lib.CalculatedColumns;
using VcdsDataPlotter.Lib.CalculatedColumns.ColumnsBuilders;
using VcdsDataPlotter.Lib.CalculatedColumns.ColumnSpecs;
using VcdsDataPlotter.Lib.CalculatedColumns.ConfigFiles;
using VcdsDataPlotter.Lib.CalculatedColumns.Math;
using VcdsDataPlotter.Lib.RawTable.Columnizer;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;
using VcdsDataPlotter.Lib.RawTableReader;
using VcdsDataPlotter.Lib.RawTableReader.Interface;

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
            result.SourceColumns = (IDiscreteDataColumn[])vcdsFile.DiscreteDataColumns.Clone();

            result.FileTime = fi.LastWriteTime;
            result.RecordingTimestamp = vcdsFile.RecordingTimestamp;

            var configRoot = AppDomain.CurrentDomain.BaseDirectory;
            var semanticColumnsIndirection = ColumnBuilderConfigurationDefinitionRoot.Load(
                Path.Combine(configRoot, "cfg", "SemanticColumns.xml"));

            List<IDiscreteDataColumn> semanticColumns = new List<IDiscreteDataColumn>();
            foreach (var definition in semanticColumnsIndirection.Columns)
            {
                var builderConfiguration = definition.Build();
                if (builderConfiguration.TryBuild(result.SourceColumns.Concat(semanticColumns).ToArray(), out var newColumn))
                {
                    semanticColumns.Add(newColumn);
                }
            }

            List<IDiscreteDataColumn> calculatedColumns = new List<IDiscreteDataColumn>();
            var calculatedColumnsIndirection = ColumnBuilderConfigurationDefinitionRoot.Load(
                Path.Combine(configRoot, "cfg", "CalculatedColumns.xml"));
            foreach (var definition in calculatedColumnsIndirection.Columns)
            {
                var builderConfiguration = definition.Build();
                if (builderConfiguration.TryBuild(result.SourceColumns.Concat(semanticColumns).Concat(calculatedColumns).ToArray(), out var newColumn))
                {
                    calculatedColumns.Add(newColumn);
                }
            }

            result.CalculatedColumns = calculatedColumns.ToArray();
            return result;
        }

        public IDiscreteDataColumn[] SourceColumns { get; set; }
        public IRawDataColumn[] RawColumns { get; set; }

        public IDiscreteDataColumn[] CalculatedColumns { get; set; }

        public DateTime FileTime { get; private set; }
        public DateTime RecordingTimestamp { get; private set; }


        private static ILogger ClassLogger = Serilog.Log.Logger.ForClass(typeof(Document));
        private ILogger? objectLogger;
        private VcdsTableColumnizer? source;
    }
}
