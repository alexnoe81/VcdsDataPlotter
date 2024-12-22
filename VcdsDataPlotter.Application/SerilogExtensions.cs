using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VcdsDataPlotter.Gui
{
    internal static class SerilogExtensions
    {
        public static ILogger ForClass(this ILogger logger, Type type) => logger.ForContext("className", type.FullName);
        public static ILogger ForMember(this ILogger logger, [CallerMemberName] string? memberName = null) => logger.ForContext("memberName", memberName);
        public static ILogger Here(this ILogger logger, [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = 0) => logger.ForContext("fileName", new System.IO.FileInfo(filePath!).Name).ForContext("lineNumber", lineNumber);
    }
}
