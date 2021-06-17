namespace Logging.Common
{
    public abstract class BaseLogEventFormatter : ILogEventFormatter
    {
        public BaseLogEventFormatter()
        {

        }
        public BaseLogEventFormatter(ILogEventFormatter logEventFormatter)
        {
            NextFormatter = logEventFormatter;
        }
        public ILogEventFormatter NextFormatter { get;}
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
