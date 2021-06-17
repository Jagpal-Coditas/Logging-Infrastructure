using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Logging.Common
{
    internal class ApplicationLogger : ILogger
    {
        private readonly ApplicationLoggerProvider _loggerProvider;
        private readonly string _sourceContext;
        public ApplicationLogger(ApplicationLoggerProvider loggerProvider, string categoryName)
        {
            _loggerProvider = loggerProvider;
            _sourceContext = categoryName;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None && (int)logLevel >= (int)_loggerProvider.Options.MinLogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel) == false)
            {
                return;
            }

            var logEvent = GetLogEvent(logLevel, eventId, state, exception, formatter);
            foreach (var logSink in _loggerProvider.Options.Sink)
            {
                logSink.Push(logEvent);
            }

        }
        LogEvent GetLogEvent<TState>(LogLevel level, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var isApiLog = false;
            var logMessage = string.Format("{0} {1}", level.ToString(), formatter(state, exception));

            var logEvent = LogEvent.Create(_loggerProvider.Options.AppName, _loggerProvider.Options.Environment, level, logMessage);
            logEvent.SourceContext = _sourceContext;
            string messageTemplate = null;
            if (state is IEnumerable<KeyValuePair<string, object>> structure)
            {
                foreach (var property in structure)
                {
                    if (property.Key == ApplicationLoggerProvider.OriginalFormatPropertyName && property.Value is string value)
                    {
                        messageTemplate = value;
                    }
                    else if (property.Key.StartsWith("@"))
                    {
                        isApiLog = property.Value is ApiLog;

                        if (property.Value != null)
                            logEvent.Properties.AddIfNotNullEmpty(property.Key.Substring(1), property.Value);
                    }
                    else
                    {
                        logEvent.Properties.AddIfNotNullEmpty(property.Key, property.Value);
                    }
                }
            }

            //LogException 
            if (exception != null)
            {
                logEvent.Exception = exception;
            }

            if (_loggerProvider.CurrentContextService != null)
            {
                foreach (var item in _loggerProvider.CurrentContextService.GetLoggerContext().Get(isApiLog))
                {
                    logEvent.Properties.AddIfNotNullEmpty(item.Key, item.Value);
                }
            }
            return logEvent;
        }
    }
}