using Coditas.Common.Logging.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Coditas.Common.Logging.Configurations
{
    public class ApplicationLoggerOptions : IApplicationLoggerOptions
    {
        public ApplicationLoggerOptions(string appName, string environment, LogLevel minLogLevel)
        {
            if (string.IsNullOrWhiteSpace(appName))
                throw new ArgumentNullException("appName");

            if (string.IsNullOrWhiteSpace(environment))
                throw new ArgumentNullException("environment");

            AppName = appName;
            Environment = environment;
            MinLogLevel = minLogLevel;
        }
        public ApplicationLoggerOptions(string appName, string environment, LogLevel minLogLevel, ICollection<ISinkService> sinks) : this(appName, environment, minLogLevel)
        {
            if (sinks == null || sinks.Count == 0)
                throw new ArgumentException("No sink added");

            Sink = sinks;
        }

        public const string LoggerOptions = "logger";
        public ICollection<ISinkService> Sink { get; }
        public string AppName { get; }
        public string Environment { get; }
        public LogLevel MinLogLevel { get; set; }
    }
}
