using Logging.Abstraction.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Logging.Abstraction.Configuration
{
    public interface IApplicationLoggerOptions
    {
        string AppName { get; }
        string Environment { get; }
        LogLevel MinLogLevel { get; set; }
        ICollection<ISinkService> Sink { get; }
    }
}
