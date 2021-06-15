using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logging.Common
{
    internal class ApplicationLogger : ILogger
    {
        private readonly ApplicationLoggerProvider _loggerProvider;

        public ApplicationLogger(ApplicationLoggerProvider loggerProvider)
        {
            _loggerProvider = loggerProvider;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel) == false)
            {
                return;
            }
            var logEvent = new LogEvent();
            try
            {

                logEvent = GetLogEvent(logLevel, eventId, state, exception, formatter);
                var logSinks = _loggerProvider.Options.Sink.Where(s => s.IsFailOverSink == false);
                foreach (var logSink in logSinks)
                {
                    logSink.Send(logEvent);
                }
            }
            catch (Exception ex)
            {
                logEvent.LoggerException = ex.StackTrace;
                logEvent.LoggerExceptionMessage = ex.Message;
                var failOverSink = _loggerProvider.Options.Sink.Where(s => s.IsFailOverSink).FirstOrDefault();
                if (failOverSink != null)
                {
                    failOverSink.Send(logEvent);
                }
            }

        }
        LogEvent GetLogEvent<TState>(LogLevel level, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var isApiLog = false;
            var logMessage = string.Format("{0} {1}", level.ToString(), formatter(state, exception));

            var logEvent = new LogEvent(_loggerProvider.Options.AppName, _loggerProvider.Options.Environment, level.ToString(), logMessage);
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
                            FlattenObjectAndAddProperties(logEvent.Properties, property.Value);
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
                logEvent.Exception = exception.StackTrace;
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
        void FlattenObjectAndAddProperties(IDictionary<string, object> propertiesList, object obj)
        {
            var jObject = JObject.Parse(JsonConvert.SerializeObject(obj));
            var flatObj = jObject.Flatten(false);
            foreach (var item in flatObj)
            {
                propertiesList.AddIfNotNullEmpty(item.Key, item.Value);
            }
        }
    }
}