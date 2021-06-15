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
        public LogEvent()
        {
            Properties = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);
            LogTime = DateTime.UtcNow;
        }
        public LogEvent(string appName, string env, string level) : this()
        {
            AppName = appName;
            Environment = env;
            Level = level;
        }
        public LogEvent(string appName, string env, string level, string message) : this(appName, env, level)
        {
            Message = message;
        }

        [JsonProperty("log_time")]
        public DateTime LogTime { get; set; }

        [JsonProperty("level")]
        public string Level { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("app_name")]
        public string AppName { get; set; }

        [JsonProperty("environment")]
        public string Environment { get; set; }

        [JsonProperty("exception")]
        public string Exception { get; set; }

        [JsonProperty("properties")]
        public IDictionary<string, object> Properties { get; set; }

        [JsonProperty("logger_exception")]
        public string LoggerException { get; set; }

        [JsonProperty("logger_exception_message")]
        public string LoggerExceptionMessage { get; set; }
    }
}