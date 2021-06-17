using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Logging.Common
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

        [JsonProperty("log_time")]
        public DateTime LogTime { get; set; }

        [JsonProperty("level")]
        public LogLevel Level { get; }

        [JsonProperty("app_name")]
        public string AppName { get; }

        [JsonProperty("environment")]
        public string Environment { get; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("exception")]
        public Exception Exception { get; set; }

        [JsonProperty("properties")]
        public IDictionary<string, object> Properties { get; set; }

        [JsonProperty("logger_exception")]
        public Exception LoggerException { get; set; }

        [JsonProperty("source_context")]
        public string SourceContext { get; set; }

        public static LogEvent Create(string appName, string env, LogLevel level, string message = null)
        {
            return new LogEvent(appName, env, level, message);
        }
    }
}