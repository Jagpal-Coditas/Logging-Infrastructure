using Coditas.Common.Logging.Models;

namespace Coditas.Common.Logging.Services
{
    public interface ILogFormatter
    {
        LogEvent Format(LogEvent logEvent);
    }
}
