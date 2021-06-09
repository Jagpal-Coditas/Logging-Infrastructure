using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logging.Common
{
    /// <summary>
    /// Global context to record all required data to be logged.
    /// </summary>
    public interface ILoggerContext
    {
        void Set(string key, string value);
        IDictionary<string, string> GetAll();
    }
}
