using Logging.Abstraction.Configuration;
using Logging.Abstraction.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace Logging.Common.Services
{
    public class ApplicationLoggerProviderService : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ApplicationLoggerService> _loggers = new ConcurrentDictionary<string, ApplicationLoggerService>();

        public readonly IApplicationLoggerOptions Options;
        public readonly ICurrentContextService CurrentContextService;
        internal const string OriginalFormatPropertyName = "{OriginalFormat}";
        public ApplicationLoggerProviderService(IApplicationLoggerOptions options, ICurrentContextService currentContextService)
        {
            if (options == null)
                throw new ArgumentNullException(typeof(IApplicationLoggerOptions).FullName);

            Options = options;
            CurrentContextService = currentContextService;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new ApplicationLoggerService(this, categoryName));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
