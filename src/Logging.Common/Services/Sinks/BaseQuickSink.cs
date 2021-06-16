namespace Logging.Common.Services.Sinks
{
    public abstract class BaseQuickSink : BaseSink
    {
        public BaseQuickSink(ILogEventPushHandler logEventPushHandler) : base(logEventPushHandler)
        {
        }
    }
}
