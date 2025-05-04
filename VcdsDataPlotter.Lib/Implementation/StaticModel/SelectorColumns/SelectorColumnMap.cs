using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VcdsDataPlotter.Lib.Implementation.StaticModel.SelectorColumns
{
    /// <summary>
    /// A selector column map defines which SelectorColumns are commonly known. For example, a SelectorColumn "NOx 1" may
    /// be defined as a column selecting either IDE04098/1 or IDE03140. This may be vehicle specific or not.
    /// 
    /// Total AdBlue consumption may be IDE16115" or "IDE03144"
    /// </summary>
    public class SelectorColumnMap
    {
        public SelectorColumnMap() { }

        public SelectorColumnStaticModel[]? SelectorColumns { get; set; }
    }

    /// <summary>
    /// Describes one virtual channel 
    /// </summary>
    public class SelectorColumnStaticModel
    {
        public SelectorColumnStaticModel() { }

        /// <summary>
        /// A readable explanation of the virtual channel
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Probably a virtual channel name, such as VIRT_NOX1
        /// </summary>
        public string ChannelId { get; set; }

        /// <summary>
        /// This may be a common implementation that just selects a value, or a specific implementation
        /// combining colums in an advanced way
        /// </summary>
        public string? TypeName { get; set; }

        public string[] SourceChannels { get; set; }
    }
}
