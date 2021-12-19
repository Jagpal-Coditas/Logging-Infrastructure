using Coditas.Common.Logging.Configurations;
using Coditas.Common.Logging.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Coditas.Common.Logging.Services
{
    internal class ApplicationLoggerService : ILogger
    {
        private readonly ILoggerContextService _loggerContextService;
        private readonly string _sourceContext;
        private readonly IApplicationLoggerOptions _options;
        public ApplicationLoggerService(ILoggerContextService loggerContextService, IApplicationLoggerOptions options, string categoryName = null)
        {
            if (options == null)
                throw new ArgumentNullException(typeof(IApplicationLoggerOptions).FullName);
            if (loggerContextService == null)
                throw new ArgumentNullException(typeof(ILoggerContextService).FullName);

            _options = options;
            _loggerContextService = loggerContextService;
            _sourceContext = categoryName;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None && (int)logLevel >= (int)_options.MinLogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel) == false)
            {
                return;
            }

            var logEvent = GetLogEvent(logLevel, eventId, state, exception, formatter);
            foreach (var logSink in _options.Sink)
            {
                logEvent.LogSink = logSink.Name;
                logSink.Send(logEvent);
            }

        }
        LogEvent GetLogEvent<TState>(LogLevel level, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var logMessage = string.Format("{0} {1}", level.ToString(), formatter(state, exception));

            var logEvent = LogEvent.Create(_options.AppName, _options.Environment, level, logMessage);
            logEvent.SourceContext = _sourceContext;
            if (state is IEnumerable<KeyValuePair<string, object>> structure)
            {
                foreach (var property in structure)
                {
                    if (property.Key.StartsWith("@"))
                    {
                        if (property.Value != null && property.Value is ApiLog)
                        {
                            logEvent = property.Value as ApiLog;
                            break;
                        }
                        if (property.Value != null)
                            logEvent.Set(property.Key.Substring(1), property.Value);
                    }
                    else
                    {
                        logEvent.Set(property.Key, property.Value);
                    }
                }
            }

            if (exception != null)
            {
                logEvent.Exception = exception;
            }

            foreach (var contextProp in _loggerContextService.Get())
            {
                logEvent.Set(contextProp.Key, contextProp.Value);
            }
            return logEvent;
        }
    }
}