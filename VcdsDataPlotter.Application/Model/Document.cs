using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VcdsDataPlotter.Lib.CalculatedColumns.ConfigFiles;
using VcdsDataPlotter.Lib.ProjectDefinition;
using VcdsDataPlotter.Lib.RawTable.Columnizer;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;
using VcdsDataPlotter.Lib.RawTableReader;
using VcdsDataPlotter.Lib.RawTableReader.Interface;

namespace VcdsDataPlotter.Gui.Model
{
    public class Document
    {
        private Document() { }

        /// <summary>
        /// Loads a VCDS recorded file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Document LoadFile(string filePath)
        {
            _ = filePath ?? throw new ArgumentNullException(nameof(filePath));

            Document result = new Document();
            result.objectLogger = ClassLogger.ForContext(nameof(filePath), filePath);
            result.objectLogger.Information("Loading file {filePath}", filePath);

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

            result.LoadSemanticColumns(Path.Combine(configRoot, "cfg", "DefaultSemanticColumns.xml"));
            result.LoadCalculatedColumns(Path.Combine(configRoot, "cfg", "DefaultCalculatedColumns.xml"));
            result.semanticColumnsDefinitionFile = new RelativePath(KnownBasePaths.DefaultConfigFiles, "DefaultSemanticColumns.xml");
            result.calculatedColumnsDefinitionFile = new RelativePath(KnownBasePaths.DefaultConfigFiles, "DefaultCalculatedColumns.xml");
            result.SourceFilePath = Path.GetFullPath(filePath);

            return result;
        }

        /// <summary>
        /// Loads semantic columns definitions from a configuration file.
        /// </summary>
        /// <param name="configfilePath"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <remarks>
        /// Theoretically, one semantic column could refer to another semantic column defined in the same file
        /// earlier.
        /// 
        /// Semantic columns are basicly renamed source columns and do not need to be displayed to the user.
        /// They may be displayed in order to show the renaming, but they do not provide more data.
        /// 
        /// When reloading semantic columns, calculated columns should also be reloaded.
        /// </remarks>
        private void LoadSemanticColumns(string configfilePath)
        {
            if (SourceColumns is null)
                throw new InvalidOperationException($"'{nameof(SourceColumns)}' must be set before calling this function.");

            var semanticColumnsIndirection = ColumnBuilderConfigurationDefinitionRoot.Load(configfilePath);
            List<IDiscreteDataColumn> semanticColumns = new List<IDiscreteDataColumn>();
            foreach (var definition in semanticColumnsIndirection.Columns)
            {
                var builderConfiguration = definition.Build();
                if (builderConfiguration.TryBuild(SourceColumns.Concat(semanticColumns).ToArray(), out var newColumn))
                {
                    semanticColumns.Add(newColumn);
                }
            }
            
            SemanticColumns = semanticColumns.ToArray();
        }

        /// <summary>
        /// Loads calculated column definitions from a configuration file
        /// </summary>
        /// <param name="configFilePath"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <remarks>
        /// Theoretically, one calculated column could refer to another calculated column defined in the same file
        /// earlier.
        /// </remarks>
        private void LoadCalculatedColumns(string configFilePath)
        {
            if (SourceColumns is null)
                throw new InvalidOperationException($"'{nameof(SourceColumns)}' must be set before calling this function.");

            var calculatedColumns = new List<IDiscreteDataColumn>();
            var calculatedColumnsIndirection = ColumnBuilderConfigurationDefinitionRoot.Load(configFilePath);

            var inputColumnsForCalculatedColumns = SourceColumns;
            if (SemanticColumns is not null)
                inputColumnsForCalculatedColumns = inputColumnsForCalculatedColumns.Concat(SemanticColumns).ToArray();

            var allAvailableColumns = new List<IDiscreteDataColumn>(inputColumnsForCalculatedColumns);

            foreach (var definition in calculatedColumnsIndirection.Columns)
            {
                var builderConfiguration = definition.Build();
                
                if (builderConfiguration.TryBuild(allAvailableColumns, out var newColumn))
                {
                    calculatedColumns.Add(newColumn!);
                    allAvailableColumns.Add(newColumn!);
                }
            }

            CalculatedColumns = calculatedColumns.ToArray();
        }


        /// <summary>
        /// Raw columns are columns from the source file without any kind of data processing, meaning that all cell contents
        /// is returned as string
        /// </summary>
        public IRawDataColumn[] RawColumns { get; set; }

        /// <summary>
        /// Sourcer columns are columns that were generated from RawColumns using a channel map.
        /// </summary>
        
        public IDiscreteDataColumn[] SourceColumns { get; set; }

        /// <summary>
        /// Semantic columns are columns which somehow exist in SourceColumns, but under different name
        /// or title. Semantic columns are used as input for calculated columns, but the UI should show
        /// the columns in SourceColumns instead.
        /// </summary>
        public IDiscreteDataColumn[] SemanticColumns { get; set; }

        /// <summary>
        /// Calculated columns are columns that do not exist in the source file, but are calculated from
        /// one or more input columns. Calculated columns use source columns, semantic columns or other
        /// calculated columns as input.
        /// </summary>
        public IDiscreteDataColumn[] CalculatedColumns { get; set; }

        public DateTime FileTime { get; private set; }
        public DateTime RecordingTimestamp { get; private set; }
        public string SourceFilePath { get; private set; }


        private FilePath semanticColumnsDefinitionFile;
        private FilePath calculatedColumnsDefinitionFile;

        private static ILogger ClassLogger = Serilog.Log.Logger.ForClass(typeof(Document));
        private ILogger? objectLogger;
        private VcdsTableColumnizer? source;
    }
}
