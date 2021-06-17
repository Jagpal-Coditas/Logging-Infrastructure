using Logging.Abstraction.Models;

namespace Logging.Abstraction.Services
{
    public interface ISinkService
    {
        ISinkService FailOverSink { get; }
        ILogEventFormatterService LogFormatter{get;}
        void Send(LogEvent logEvent);
    }
}
