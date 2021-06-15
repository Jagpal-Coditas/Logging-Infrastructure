using System;
using System.Collections.Generic;
using System.Linq;

namespace Logging.Common
{
    public class ApplicationLoggerOptions : IApplicationLoggerOptions
    {
        private ApplicationLoggerOptions(string appName, string environment)
        {
            if (string.IsNullOrWhiteSpace(appName))
                throw new ArgumentNullException("appName");

            if (string.IsNullOrWhiteSpace(environment))
                throw new ArgumentNullException("environment");

            AppName = appName;
            Environment = environment;
        }
        private ApplicationLoggerOptions(string appName, string environment, ICollection<ISink> sinks) : this(appName, environment)
        {
            if (sinks == null)
                Sink = new List<ISink>();

            if (sinks.Count > 0 && sinks.All(s => s.IsFailOverSink))
                throw new ArgumentException("All Sinks are FailOverSink, At least one non FailOverSink required");

            Sink = sinks;
        }
        public ICollection<ISink> Sink { get; }
        public string AppName { get; }
        public string Environment { get; }
        public static IApplicationLoggerOptions Create(string appName, string environment)
        {
            return new ApplicationLoggerOptions(appName, environment);
        }
        public static IApplicationLoggerOptions Create(string appName, string environment, ICollection<ISink> sinks)
        {
            return new ApplicationLoggerOptions(appName, environment, sinks);
        }
    }
}
