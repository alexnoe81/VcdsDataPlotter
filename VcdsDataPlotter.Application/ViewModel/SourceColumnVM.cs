using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VcdsDataPlotter.Gui.ViewModel.Base;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Gui.ViewModel
{
    public class ChangedEventArgs<T> : EventArgs
    {
        public ChangedEventArgs(T oldValue, T newValue) => (OldValue, NewValue) = (oldValue, newValue);
        public T OldValue { get; private set; }
        public T NewValue { get; private set; }
    }

    public abstract class RenderableColumnVM : DataColumnVM
    {
        protected RenderableColumnVM(IDiscreteDataColumn model) : base(model)
        {
        }

        public bool IsSelected
        {
            get => this.isSelected;
            set => SetProperty(ref this.isSelected, value, (oldValue, newValue) => IsSelectedChanged?.Invoke(this, new ChangedEventArgs<bool>(oldValue, newValue)));
        }

        public string? Title
        {
            get => this.sourceColumnTitle;
            set => SetProperty(ref this.sourceColumnTitle, value);
        }

        public string? Unit
        {
            get => this.sourceColumnUnit;
            set => SetProperty(ref this.sourceColumnUnit, value);
        }

        public override IEnumerable<SingleDataItem> EnumeratePoints()
        {
            foreach (var item in Model.EnumerateDataItems())
                yield return item;
        }

        public event EventHandler<ChangedEventArgs<bool>>? IsSelectedChanged;

        private string? sourceColumnTitle;
        private string? sourceColumnUnit;
        private bool isSelected;
    }

    /// <summary>
    /// This represents a column that does not use any other column as input
    /// </summary>
    public class SourceColumnVM : RenderableColumnVM
    {
        public SourceColumnVM(IDiscreteDataColumn model) : base(model) { }

        /// <summary>
        /// Returns the ID of the source column. This may be null for virtual columns
        /// </summary>
        public string? Id
        {
            get => this.id;
            set => SetProperty(ref this.id, value);
        }



        private string? id;

    }

    public class CalculatedColumnVM : RenderableColumnVM
    {
        public CalculatedColumnVM(IDiscreteDataColumn model) : base(model)
        {
        }
    }
}
