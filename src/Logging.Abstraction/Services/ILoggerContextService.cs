using Logging.Abstraction.Models;
using System.Collections.Generic;

namespace Logging.Abstraction.Services
{
    /// <summary>
    /// Global context to record all required data to be logged.
    /// </summary>
    public interface ILoggerContextService
    {
        void Set(LogContextProperty prop);
        IEnumerable<LogContextProperty> Get(bool IsMiddlewareLogging = false);
    }
}
