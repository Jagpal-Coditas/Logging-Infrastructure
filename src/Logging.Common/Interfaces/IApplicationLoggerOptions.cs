using System.Collections.Generic;

namespace Logging.Common
{
    public interface IApplicationLoggerOptions
    {
        string AppName { get; }
        string Environment { get; }
        ICollection<ISink> Sink { get; }
    }
}
