using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logging.Common.Services
{
    public abstract class BaseSink<T> : ISink<T>
    {
        private readonly ILogEventPushHandler<T> _logEventPushHander;

        public BaseSink(ILogEventPushHandler<T> logEventPushHandler)
        {
            _logEventPushHander = logEventPushHandler;
        }

        public abstract bool IsFailOverSink { get; set; }

        public bool Push(LogEvent logEvent)
        {
            return _logEventPushHander.AddOrPush(logEvent, PushToStore);
        }

        public abstract Task<bool> PushToStore(T logBatch);
    }
}
