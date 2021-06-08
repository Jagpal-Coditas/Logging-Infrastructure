using System.Threading.Tasks;

namespace Logging.Common
{
    public interface ILoggerContext
    {
        Task Set(string key, string value);
    }
}
