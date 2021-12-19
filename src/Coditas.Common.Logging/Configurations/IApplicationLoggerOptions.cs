using Coditas.Common.Logging.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Coditas.Common.Logging.Configurations
{
    public interface IApplicationLoggerOptions
    {
        string AppName { get; }
        string Environment { get; }
        LogLevel MinLogLevel { get; set; }
        ICollection<ISinkService> Sink { get; }
    }
}
