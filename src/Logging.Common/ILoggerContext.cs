using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logging.Common
{
    public interface ILoggerContext
    {
        Task Set(string key, string value);
        IDictionary<string, string> GetAll();
    }
}
