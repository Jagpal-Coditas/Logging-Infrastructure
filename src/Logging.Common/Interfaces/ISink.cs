namespace Logging.Common
{
    public interface ISink
    {
        bool IsPrioritySink { get; }
        bool IsFailOverSink { get; }
        void Push(LogEvent logEvent);
    }
}
