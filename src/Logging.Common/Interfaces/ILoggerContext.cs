using System.Collections.Generic;

namespace Logging.Common
{
    /// <summary>
    /// Global context to record all required data to be logged.
    /// </summary>
    public interface ILoggerContext
    {
        void Set(LogContextProperty prop);
        IEnumerable<LogContextProperty> Get(bool IsMiddlewareLogging = false);
    }
}
