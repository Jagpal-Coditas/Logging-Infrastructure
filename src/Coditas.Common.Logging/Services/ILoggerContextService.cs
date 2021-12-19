using System.Collections.Generic;

namespace Coditas.Common.Logging.Services
{
    /// <summary>
    /// Global context to record all required data to be logged.
    /// </summary>
    public interface ILoggerContextService
    {
        void Set(string key, object value);
        IReadOnlyDictionary<string, object> Get();
    }
}
