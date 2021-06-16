using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logging.Common.Services
{
    public abstract class BaseSink : ISink
    {
        private readonly ILogEventPushHandler _logEventPushHander;

        public BaseSink(ILogEventPushHandler logEventPushHandler)
        {
            _logEventPushHander = logEventPushHandler;
        }

        public abstract bool IsFailOverSink { get; set; }

        public bool Push(LogEvent logEvent)
        {
            return _logEventPushHander.AddOrPush(logEvent, PushToStore);
        }

        public Task<bool> PushToStore(LogEvent logBatch)
        {
        }

        public Task<bool> PushToStore(IEnumerable<LogEvent> logBatch)
        {
        }
    }
}
