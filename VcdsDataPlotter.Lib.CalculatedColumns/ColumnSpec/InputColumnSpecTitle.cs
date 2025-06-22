using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VcdsDataPlotter.Lib.RawTable.Columnizer.Interface;

namespace VcdsDataPlotter.Lib.CalculatedColumns.ColumnSpec
{
    internal class InputColumnSpecTitle : MatchableInputColumnSpec
    {
        public InputColumnSpecTitle(string title)
        {
            this.title = title ?? throw new ArgumentNullException(nameof(title));
            this.normalizedTitle = NormalizeTitle(title);
        }

        public override bool Matches(IDiscreteDataColumn comparee)
        {
            _ = comparee ?? throw new ArgumentNullException(nameof(comparee));

            return NormalizeTitle(comparee.Title ?? "").Contains(normalizedTitle);
        }

        private string NormalizeTitle(string title)
        {
            // Might want to remove more characters, like newline, linefeed, hyphen...
            return title.Replace(" ", "").ToLowerInvariant();
        }


        private string title;
        private string normalizedTitle;
    }
}
