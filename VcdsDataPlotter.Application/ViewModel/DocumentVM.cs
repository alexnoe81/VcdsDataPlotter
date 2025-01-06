using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Gui.Model;
using VcdsDataPlotter.Gui.ViewModel.Base;

namespace VcdsDataPlotter.Gui.ViewModel
{
    public class DocumentVM : ViewModelBase
    {
        private DocumentVM(string filePath)
        {
            this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            objectLogger = ClassLogger.ForContext(nameof(filePath), filePath);
        }

        public void Initialize()
        {
            Document document = Document.LoadFile(filePath);
            this.document = document;

            foreach (var column in document.DiscreteColumns)
            {
                DataColumnVM newColumn = new DataColumnVM(column)
                {
                    SourceColumnId = column.ChannelId,
                    SourceColumnTitle = column.Title,
                    SourceColumnUnit = column.Unit
                };

                dataColumns.Add(newColumn);
            }
        }

        public static DocumentVM LoadFile(string filePath)
        {
            var result = new DocumentVM(filePath);
            result.Initialize();
            return result;
        }

        public string FilePath
        {
            get => this.filePath;
            set => SetProperty(ref this.filePath, value);
        }

        public DateTime RecordingTimestamp => this.document.RecordingTimestamp;
        

        public ObservableCollection<DataColumnVM> DataColumns { get => dataColumns; }


        private static ILogger ClassLogger = Serilog.Log.Logger.ForClass(typeof(DocumentVM));
        private ILogger? objectLogger;
        private Document? document;

        private ObservableCollection<DataColumnVM> dataColumns = new ();
        private string filePath;
    }
}
