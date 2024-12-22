using Serilog.Formatting.Compact;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Exceptions;

namespace VcdsDataPlotter.Gui;
static class SerilogInitializer
{
    public static void Initialize()
    {
        LoggerConfiguration config = new LoggerConfiguration()
            .Enrich.WithThreadId()
            .Enrich.WithExceptionDetails()
            .WriteTo.File(
                path: @"Logfiles\VcdsDataPlotter-",
                rollingInterval: RollingInterval.Day,
                formatter: new CompactJsonFormatter(),
                encoding: Encoding.UTF8);

        Serilog.Log.Logger = config.CreateLogger();
        Serilog.Log.Logger.Information("Logger initialized.");
    }
}
