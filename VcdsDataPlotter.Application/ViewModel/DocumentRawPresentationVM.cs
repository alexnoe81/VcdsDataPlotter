using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using VcdsDataPlotter.Gui.Model;
using VcdsDataPlotter.Gui.ViewModel.Base;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Gui.ViewModel
{
    // Probably not strictly needed
    public class DocumentRawPresentationWindowVM : ViewModelBase
    {
        public DocumentRawPresentationWindowVM() { }

        public DocumentRawPresentationVM Data
        {
            get => this.data;
            set => SetProperty(ref this.data, value, OnDataChanged);
        }

        private void OnDataChanged(DocumentRawPresentationVM oldValue, DocumentRawPresentationVM newValue)
        {

        }

        private DocumentRawPresentationVM data;
    }

    /// <summary>
    /// View model for raw data presentation. This does not include a possible "Close" button in a possible window.
    /// </summary>
    public class DocumentRawPresentationVM : DocumentVMBase
    {
        private DocumentRawPresentationVM(Document document) 
            : base(document)
        {
        }

        public event EventHandler<EventArgs> SelectedColumnsChanged;

        public static DocumentRawPresentationVM Load(Document document)
        {
            var result = new DocumentRawPresentationVM(document);
            result.Initialize();

            return result;
        }

        private void Initialize()
        {
            InitializeSourceColumns();

            foreach (var item in SourceColumns)
                item.IsSelected = true;

            foreach (var sourceColumn in SourceColumns)
                WeakEventManager<RenderableColumnVM, EventArgs>.AddHandler(
                    sourceColumn, nameof(sourceColumn.IsSelectedChanged), OnColumnIsSelectedChanged);

            RebuildDataTable();
        }

        private void OnColumnIsSelectedChanged(object? sender, EventArgs e)
        {
            SelectedColumnsChanged?.Invoke(this, EventArgs.Empty);
        
            RebuildDataTable();
        }

        private void RebuildDataTable()
        {
            DataTable newDataTable = new();

            columnsDisplaySettings.Clear();
            newDataTable.Columns.Add(new DataColumn() { Caption = "Time [s]", DataType = typeof(TimeSpan), ColumnName = "col_0" });
            var selectedColumns = SourceColumns.Where(item => item.IsSelected).ToArray();
            columnsDisplaySettings["col_0"] = new ColumnInfo() { DisplayName = "Time [s]", RecommendedStringFormat = "hh\\:mm\\:ss\\.fff" };

            var index = 1;
            foreach (var sourceColumn in selectedColumns)
            {
                var newColumn = new DataColumn();
                newColumn.ColumnName = "col_" + index++.ToString();
                newColumn.Caption = sourceColumn.Title + "\n" + sourceColumn.Unit;
                newColumn.DataType = typeof(object);

                columnsDisplaySettings[newColumn.ColumnName] = new ColumnInfo() 
                {
                    DisplayName = sourceColumn.Title + "\n" + sourceColumn.Unit,
                    RecommendedStringFormat = "0.####" 
                };

                newDataTable.Columns.Add(newColumn);
            }

            // Now we build a queue for each source column. Basicly, for each column we need to know if it belongs
            // to the current row, or if we have to put it onto the next one

            Queue<SingleDataItem>[] queues = new Queue<SingleDataItem>[selectedColumns.Length];
            for (int j = 0; j < queues.Length; j++)
                queues[j] = new();

            IEnumerator<SingleDataItem>[] enumerators = new IEnumerator<SingleDataItem>[selectedColumns.Length];
            for (int j = 0; j < selectedColumns.Length; j++)
                enumerators[j] = selectedColumns[j].EnumeratePoints().GetEnumerator();

            while (true)
            {
                // Make sure we have two datapoints in every queue, if two data points are available
                for (int j = 0; j < selectedColumns.Length; j++)
                {
                    while (queues[j].Count < 2)
                    {
                        if (!enumerators[j].MoveNext()) break;
                        queues[j].Enqueue(enumerators[j].Current);
                    }
                }

                var governingQueueIndex = -1;
                for (int j = 0; j < selectedColumns.Length; j++)
                {
                    if (queues[j].Count >= 2)
                    {
                        governingQueueIndex = j;
                        break;
                    }
                }

                if (governingQueueIndex == -1)
                    break;

                var maxTimeStampForCurrentRow = queues[governingQueueIndex].Last().TimeStamp;
                List<object> dataItemsForNextRow = [queues[governingQueueIndex].First().TimeStamp];

                for (int j = 0; j < selectedColumns.Length; j++)
                {
                    if (queues[j].First().TimeStamp < maxTimeStampForCurrentRow)
                    {
                        dataItemsForNextRow.Add(queues[j].Dequeue().RawData);
                    }
                    else
                        dataItemsForNextRow.Add(null);
                }

                newDataTable.Rows.Add(dataItemsForNextRow.ToArray());
            }

            ActiveDataTable = newDataTable;

            // TODO: Add actual data
            // TODO: Find out if we need this raw view in the first place
        }


        public DataTable ActiveDataTable 
        {
            get => this.activeDataTable;
            private set => SetProperty(ref this.activeDataTable, value);
        }

        public IDictionary<string, ColumnInfo> ColumnsDisplaySettings => this.columnsDisplaySettings;

        private DataTable activeDataTable;
        private Dictionary<string, ColumnInfo> columnsDisplaySettings = new();
        private static ILogger ClassLogger = Serilog.Log.Logger.ForClass(typeof(DocumentRawPresentationVM));
        private ILogger? objectLogger;
    }

   /* public class TableDataVM : ICustomTypeDescriptor
    {
        public List<string> properties;

        public List<List<object>> data;



        public AttributeCollection GetAttributes()
        {
            throw new NotImplementedException();
        }

        public string? GetClassName()
        {
            throw new NotImplementedException();
        }

        public string? GetComponentName()
        {
            throw new NotImplementedException();
        }

        public TypeConverter GetConverter()
        {
            throw new NotImplementedException();
        }

        public EventDescriptor? GetDefaultEvent()
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptor? GetDefaultProperty()
        {
            throw new NotImplementedException();
        }

        public object? GetEditor(Type editorBaseType)
        {
            throw new NotImplementedException();
        }

        public EventDescriptorCollection GetEvents()
        {
            throw new NotImplementedException();
        }

        public EventDescriptorCollection GetEvents(Attribute[]? attributes)
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            PropertyDescriptorCollection result = new PropertyDescriptorCollection();

            throw new NotImplementedException();
        }

        public PropertyDescriptorCollection GetProperties(Attribute[]? attributes)
        {
            throw new NotImplementedException();
        }

        public object? GetPropertyOwner(PropertyDescriptor? pd)
        {
            throw new NotImplementedException();
        }
    }
   */
    public class ColumnInfo
    {
        public string DisplayName { get; set; }
        public string RecommendedStringFormat { get; set; }
    }
}
