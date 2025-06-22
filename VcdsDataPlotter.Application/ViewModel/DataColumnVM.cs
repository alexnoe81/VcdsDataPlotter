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
    /// This represents a column that can provide data points. 
    /// </summary>
    public abstract class DataColumnVM : ViewModelBase
    {
        protected DataColumnVM(IDiscreteDataColumn model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }


        /// <summary>
        /// Allows accessing the model for this column
        /// </summary>
        public IDiscreteDataColumn Model { get; protected set; }


        public abstract IEnumerable<SingleDataItem> EnumeratePoints();
    }
}
