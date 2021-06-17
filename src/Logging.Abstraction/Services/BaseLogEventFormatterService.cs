using Logging.Abstraction.Models;

namespace Logging.Abstraction.Services
{
    public abstract class BaseLogEventFormatterService : ILogEventFormatterService
    {
        public BaseLogEventFormatterService()
        {

        }
        public BaseLogEventFormatterService(ILogEventFormatterService logEventFormatter)
        {
            NextFormatter = logEventFormatter;
        }
        public ILogEventFormatterService NextFormatter { get;}
        public LogEvent Format(LogEvent logEvent)
        {
            if (logEvent == null || logEvent.Properties == null)
                return logEvent;

            if (NextFormatter != null)
                return NextFormatter.Format(DoFormatting(logEvent));
            else
                return DoFormatting(logEvent);
        }
        protected abstract LogEvent DoFormatting(LogEvent logEvent);
    }
}
