using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Coditas.Common.Logging.Models
{
    /// <summary>
    /// Necessary logging fields across all applications.
    /// </summary>
    public class LogEvent
    {
        private readonly ConcurrentDictionary<string, object> _loggingFields = new ConcurrentDictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);
        private LogEvent()
        {
            LogTime = DateTime.UtcNow;
        }
        protected LogEvent(string appName, string env) : this()
        {
            AppName = appName;
            Environment = env;
        }
        protected LogEvent(string appName, string env, LogLevel level, string message) : this(appName, env)
        {
            Level = level;
            Message = message;
        }
        public DateTime LogTime { get; set; }
        public LogLevel Level { get; }
        public string AppName { get; }
        public string Environment { get; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public IReadOnlyDictionary<string, object> LoggingFields
        {
            get
            {
                return (IReadOnlyDictionary<string, object>)_loggingFields;
            }
        }
        public Exception LoggerException { get; set; }
        public string SourceContext { get; set; }
        public string LogSink { get; set; }

        public static LogEvent Create(string appName, string env, LogLevel level, string message = null)
        {
            return new LogEvent(appName, env, level, message);
        }

        public void Set(string name, object value)
        {
            if (string.IsNullOrWhiteSpace(name) && value == null)
                return;
            var fields = Utility.GetFormattedKeyValues(name, value);
            foreach (var field in fields)
            {
                _loggingFields.AddOrUpdate(field.Key, field.Value, (currentKey, currentValue) => field.Value);
            }
        }
    }
}