using Microsoft.Extensions.Logging;
using System;

namespace Logging.Common
{
    public class ApplicationLoggerProvider : ILoggerProvider
    {
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
            return new ApplicationLogger(this);
        }

        public void Dispose()
        {

        }
    }
}
