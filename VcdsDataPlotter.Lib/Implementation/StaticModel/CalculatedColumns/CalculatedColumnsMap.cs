using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.Implementation.StaticModel.SelectorColumns;

namespace VcdsDataPlotter.Lib.Implementation.StaticModel.CalculatedColumns
{
    public class CalculatedColumnsMap
    {
        public CalculatedColumnsMap() { }

        public CalculatedColumnStaticModel[]? Models { get; set; }
    }

    /// <summary>
    /// The static model of a calculated column declares a type of column. Column types could be:
    /// - Accumulated mass flow, which would be the integral over the mass flow
    /// - Average of a value, like average mass flow per second with rolling average over one minute
    /// </summary>
    /// Every calculated column needs one or more input columns, and possibly needs a couple of fixed
    /// or editable parameters. A column calculating a rolling average, for example, needs to know for
    /// how much data to calculate its rolling average.
    ///
    public class CalculatedColumnStaticModel
    {
        // Required sources
        public CalculatedColumnInputStaticModel[] Input { get; set; }


        // Required parameters
        public CalculatedColumnParameterStaticModel[] Arguments { get; set; }
    }

    public class CalculatedColumnInputStaticModel
    {
        public string Name { get; private set; }

        public string Explanation { get; private set; }
    }


    public class CalculatedColumnParameterStaticModel
    {

        public string Name { get; private set; }

        /// <summary>
        /// Probably always double. Probably needed to editing
        /// </summary>
        public Type Type { get; private set; }


        public string Explanation { get; private set; }

        public double? MinValue { get; private set; }
        public double? MaxValue { get; private set; }
    }

}
