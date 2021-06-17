namespace Logging.Common
{
    public interface ISink
    {
        ILogEventFormatter LogFormatter{get;}
        bool IsPrioritySink { get; }
        bool IsFailOverSink { get; }
        void Push(LogEvent logEvent);
    }
}
