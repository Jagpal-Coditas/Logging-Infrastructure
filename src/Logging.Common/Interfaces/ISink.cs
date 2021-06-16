namespace Logging.Common
{
    public interface ISink
    {
        ISink FailOverSink { get; }
        ILogEventFormatter LogFormatter{get;}
        void Send(LogEvent logEvent);
    }
}
