namespace Logging.Common
{
    public interface ILogEventFormatter
    {
        ILogEventFormatter NextFormatter { get; }
        LogEvent Format(LogEvent logEvent);
    }
}
