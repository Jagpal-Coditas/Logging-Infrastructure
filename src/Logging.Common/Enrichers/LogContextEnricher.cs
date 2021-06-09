using Serilog.Core;
using Serilog.Events;
using System;

namespace Logging.Common.Enrichers
{
    public class LogContextEnricher : ILogEventEnricher
    {
        private readonly ILoggerContext _loggerContext;
        public LogContextEnricher(ILoggerContext loggerContext)
        {
            if (loggerContext == null)
                throw new ArgumentNullException(typeof(ILoggerContext).FullName);
            _loggerContext = loggerContext;
        }
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            LogEventPropertyValue logEventPropertyValue;
            if (logEvent.Properties.ContainsKey("IsLoggedFromMiddleware"))
            {
                logEvent.Properties.TryGetValue("IsLoggedFromMiddleware", out logEventPropertyValue);
                var propScalarValue = logEventPropertyValue as ScalarValue;
                if (Convert.ToBoolean(propScalarValue.Value))
                {
                    foreach (var keyValue in _loggerContext.GetAll())
                    {
                        logEvent.AddOrUpdateProperty(new LogEventProperty(keyValue.Key, new ScalarValue(keyValue.Value)));
                    }
                }
            }
        }
    }
}
