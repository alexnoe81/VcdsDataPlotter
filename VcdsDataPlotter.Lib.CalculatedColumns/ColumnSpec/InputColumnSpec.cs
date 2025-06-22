using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.CalculatedColumns.ColumnSelection;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.CalculatedColumns.ColumnSpec
{
    /// <summary>
    /// An InputColumnSpec defines search criteria to find a column, such as "Find columns where the channel id 
    /// is IDE00123" or "Find columns where the channel title contains "Vehicle speed". InputColumnSpecs are used
    /// to define calculated columns that need input of a certain type. For example, a calculated column that calculates
    /// the traveled distanced based on the current vehicle speed needs to be constructed from a column that contains
    /// the current vehicle speed.
    /// </summary>
    public abstract class InputColumnSpec
    {
        protected InputColumnSpec() { }
        public abstract bool Matches(IDiscreteDataColumn comparee);
        public static MatchableInputColumnSpec ChannelIdIs(string channelId) => new InputColumnSpecChannelId(channelId);
        public static MatchableInputColumnSpec TitleContains(string title) => new InputColumnSpecTitle(title);
    }

    public static class ColumnSelectors
    {
        public static ColumnSelector Select(MatchableInputColumnSpec spec) => new SelectFirst(spec);
        public static ColumnSelector SelectFirst(params MatchableInputColumnSpec[] specs) => new SelectFirst(specs);
    }
}
