using Logging.Common.Configurations;
using Logging.Common.Enrichers;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;

namespace Logging.Infrastructure
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

            this.LoggerConfiguration = new LoggerConfiguration()
                .Enrich.WithProperty(Common.Constants.ENVIRONMENT, serilogOptions.Environment)
                .Enrich.WithProperty(Common.Constants.APPLICATIONNAME, serilogOptions.AppName)
                .Enrich.With(new LogContextEnricher(serilogOptions.GetLoggerContext())).WriteTo.Sink(logEventSink, level);

        }
        public LoggerConfiguration LoggerConfiguration { get; private set; }
    }
}
