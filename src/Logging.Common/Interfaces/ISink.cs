namespace Logging.Common
{
    public interface ISink
    {
        bool IsFailOverSink { get; set; }
        void Send(LogEvent logEvent);
    }
}
