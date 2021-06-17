using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Logging.Abstraction.Models
{
    /// <summary>
    /// Necessary logging fields across all applications.
    /// </summary>
    public class LogEvent
    {
        private LogEvent()
        {
            Properties = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);
            LogTime = DateTime.UtcNow;
        }
        private LogEvent(string appName, string env, LogLevel level, string message) : this()
        {
            AppName = appName;
            Environment = env;
            Level = level;
            Message = message;
        }
        public DateTime LogTime { get; set; }
        public LogLevel Level { get; }
        public string AppName { get; }
        public string Environment { get; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public IDictionary<string, object> Properties { get; set; }
        public Exception LoggerException { get; set; }
        public string SourceContext { get; set; }

        public static LogEvent Create(string appName, string env, LogLevel level, string message = null)
        {
            return new LogEvent(appName, env, level, message);
        }
    }
}