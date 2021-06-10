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
            bool isLoggedFromMiddleware = false;
            if (logEvent.Properties.ContainsKey("IsLoggedFromMiddleware"))
            {
                LogEventPropertyValue logEventPropertyValue;
                logEvent.Properties.TryGetValue("IsLoggedFromMiddleware", out logEventPropertyValue);
                var propScalarValue = logEventPropertyValue as ScalarValue;
                isLoggedFromMiddleware = Convert.ToBoolean(propScalarValue.Value);
            }

            foreach (var keyValue in _loggerContext.Get(isLoggedFromMiddleware))
            {
                logEvent.AddOrUpdateProperty(new LogEventProperty(keyValue.Key, new ScalarValue(keyValue.Value)));
            }
        }
    }
}
