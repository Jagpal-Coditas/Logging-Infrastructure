namespace Logging.Common.Services.Sinks
{
    public abstract class BaseQuickSink : BaseSink<LogEvent>
    {
        public BaseQuickSink(ILogEventPushHandler<LogEvent> logEventPushHandler) : base(logEventPushHandler)
        {
        }
    }
}
