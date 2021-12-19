using Coditas.Common.Logging.Configurations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace Coditas.Common.Logging.Services
{
    public class ApplicationLoggerProviderService : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ApplicationLoggerService> _loggers = new ConcurrentDictionary<string, ApplicationLoggerService>();

        public readonly IApplicationLoggerOptions _options;
        private readonly ILoggerContextService _loggerContextService;
        public ApplicationLoggerProviderService(IApplicationLoggerOptions options, ILoggerContextService loggerContextService)
        {
            if (options == null)
                throw new ArgumentNullException(typeof(IApplicationLoggerOptions).FullName);
            if (loggerContextService == null)
                throw new ArgumentNullException(typeof(ILoggerContextService).FullName);

            _options = options;
            _loggerContextService = loggerContextService;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new ApplicationLoggerService(_loggerContextService, _options, categoryName));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
