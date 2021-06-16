using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace Logging.Common
{
    public class ApplicationLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ApplicationLogger> _loggers = new ConcurrentDictionary<string, ApplicationLogger>();

        public readonly IApplicationLoggerOptions Options;
        public readonly ICurrentContextService CurrentContextService;
        internal const string OriginalFormatPropertyName = "{OriginalFormat}";
        public ApplicationLoggerProvider(IApplicationLoggerOptions options, ICurrentContextService currentContextService)
        {
            if (options == null)
                throw new ArgumentNullException(typeof(IApplicationLoggerOptions).FullName);

            Options = options;
            CurrentContextService = currentContextService;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new ApplicationLogger(this, categoryName));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
