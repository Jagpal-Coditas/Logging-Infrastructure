using Logging.Abstraction.Models;

namespace Logging.Abstraction.Services
{
    public interface ILogEventFormatterService
    {
        ILogEventFormatterService NextFormatter { get; }
        LogEvent Format(LogEvent logEvent);
    }
}
