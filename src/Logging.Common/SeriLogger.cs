using Logging.Common.Configurations;
using Logging.Common.Enrichers;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;

namespace Logging.Common
{
    public sealed class SeriLogger
    {
        public SeriLogger(SerilogOptions serilogOptions, ILogEventSink logEventSink)
        {
            LogEventLevel level;
            if (!Enum.TryParse<LogEventLevel>(serilogOptions.Level, true, out level))
            {
                level = LogEventLevel.Information;
            }

            //var basePath = Path.GetFullPath(serilogOptions.LogFilePath + "/" + serilogOptions.AppName);
            //if (!Directory.Exists(basePath))
            //    Directory.CreateDirectory(basePath);
            //var file = File.CreateText(basePath + "/failures.txt");
            //SelfLog.Enable(TextWriter.Synchronized(file));

            this.LoggerConfiguration = new LoggerConfiguration().WriteTo.Sink(logEventSink, level);
            
        }
        public LoggerConfiguration LoggerConfiguration { get; private set; }
    }
}
