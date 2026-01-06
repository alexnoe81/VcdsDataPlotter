using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VcdsDataPlotter.Gui.Model;
using VcdsDataPlotter.Gui.ViewModel.Base;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Gui.ViewModel
{
    public class DocumentVMBase : ViewModelBase
    {
        protected DocumentVMBase(Document document)
        {
            this.document = document ?? throw new ArgumentNullException(nameof(document));
            this.filePath = document.SourceFilePath;
        }

        protected void InitializeSourceColumns()
        {
            foreach (var column in document.SourceColumns.Concat(document.CalculatedColumns ?? Array.Empty<IDiscreteDataColumn>()))
            {
                SourceColumnVM newColumn = new SourceColumnVM(column)
                {
                    Id = column.ChannelId,
                    Title = column.Title,
                    Unit = column.Unit
                };

                sourceColumns.Add(newColumn);
            }
        }
        public string FilePath
        {
            get => this.filePath;
            set => SetProperty(ref this.filePath, value);
        }

        public DateTime RecordingTimestamp => Document.RecordingTimestamp;

        public Document Document => this.document;

        /// <summary>
        /// Contains columns that are available directly in the source file
        /// </summary>
        public ObservableCollection<SourceColumnVM> SourceColumns { get => sourceColumns; }

        private ObservableCollection<SourceColumnVM> sourceColumns = new();

        private Document document;
        private string filePath;
    }

    public class DocumentOverviewPresentationVM : DocumentVMBase
    {
        private DocumentOverviewPresentationVM(Document document)
            : base(document)
        {
            objectLogger = ClassLogger.ForContext("filePath", document.SourceFilePath);

            CmdAddColumn = new SimpleCommand(DoCmdAddColumn);
        }

        public event EventHandler<EventArgs> SelectedColumnsChanged;


        public ICommand CmdAddColumn { get; private set; }

        public event EventHandler<EventArgs>? OnCmdAddColumn;

        private void DoCmdAddColumn() => OnCmdAddColumn?.Invoke(this, EventArgs.Empty);

        public void Initialize()
        {
            InitializeSourceColumns();
            foreach (var item in SourceColumns)
                WeakEventManager<RenderableColumnVM, EventArgs>.AddHandler(
                    item, nameof(item.IsSelectedChanged), OnColumnIsSelectedChanged);

            foreach (var column in Document.CalculatedColumns ?? Array.Empty<IDiscreteDataColumn>())
            {
                RenderableColumnVM newColumn = new CalculatedColumnVM(column)
                {
                    Title = column.Title,
                    Unit = column.Unit
                };

                calculatedColumns.Add(newColumn);
                WeakEventManager<RenderableColumnVM, EventArgs>.AddHandler(
                    newColumn, nameof(newColumn.IsSelectedChanged), OnColumnIsSelectedChanged);
            }
        }

        private void OnColumnIsSelectedChanged(object? sender, EventArgs e)
        {
            SelectedColumnsChanged?.Invoke(this, EventArgs.Empty);
        }

        public static DocumentOverviewPresentationVM LoadFile(string filePath)
        {
            Document document = Document.LoadFile(filePath);
            var result = new DocumentOverviewPresentationVM(document);

            result.FilePath = filePath;
            result.Initialize();

            return result;
        }

        public static DocumentOverviewPresentationVM Load(Document document)
        {
            var result = new DocumentOverviewPresentationVM(document);
            result.FilePath = document.SourceFilePath;
            result.Initialize();

            return result;
        }




        /// <summary>
        /// Contains columns that are not directly available in the source file, but are
        /// directly calculated from the source file without user interaction, without
        /// users providing parameters etc.
        /// </summary>
        public ObservableCollection<RenderableColumnVM> CalculatedColumns { get => calculatedColumns; }


        private static ILogger ClassLogger = Serilog.Log.Logger.ForClass(typeof(DocumentOverviewPresentationVM));
        private ILogger? objectLogger;

        private ObservableCollection<RenderableColumnVM> calculatedColumns = new ();

    }
}
