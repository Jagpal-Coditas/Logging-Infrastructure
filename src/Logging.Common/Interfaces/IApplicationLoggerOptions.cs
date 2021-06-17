using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Logging.Common
{
    public interface IApplicationLoggerOptions
    {
        string AppName { get; }
        string Environment { get; }
        LogLevel MinLogLevel { get; set; }
        ICollection<ISink> Sink { get; }
    }
}
