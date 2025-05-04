using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VcdsDataPlotter.Gui.ViewModel.Base;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Gui.ViewModel
{
    /// <summary>
    /// This represents a column that does not use any other column as input
    /// </summary>
    public class SourceColumnVM : DataColumnVM
    {
        public SourceColumnVM(IDiscreteDataColumn model) : base(model) { }

        /// <summary>
        /// Returns the ID of the source column. This may be null for virtual columns
        /// </summary>
        public string? SourceColumnId
        {
            get => this.sourceColumnId;
            set => SetProperty(ref this.sourceColumnId, value);
        }

        public string? SourceColumnTitle
        {
            get => this.sourceColumnTitle;
            set => SetProperty(ref this.sourceColumnTitle, value);
        }

        public string? SourceColumnUnit
        {
            get => this.sourceColumnUnit;
            set => SetProperty(ref this.sourceColumnUnit, value);
        }

        public override IEnumerable<SingleDataItem> EnumeratePoints()
        {
            foreach (var item in Model.EnumerateDataItems())
                yield return item;
        }

        private string? sourceColumnId;
        private string? sourceColumnTitle;
        private string? sourceColumnUnit;
    }
}
