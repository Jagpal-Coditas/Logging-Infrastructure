using System.Collections.Generic;

namespace Logging.Common.Services
{
    public abstract class BaseSink : ISink
    {
        public abstract ILogEventFormatter LogFormatter { get; }
        public abstract bool IsFailOverSink { get; }

        public abstract bool IsPrioritySink { get; }
        
        private bool IsSinkEligible(LogEvent logEvent)
        {
            return logEvent.Level != Microsoft.Extensions.Logging.LogLevel.Critical || (logEvent.Level == Microsoft.Extensions.Logging.LogLevel.Critical && IsPrioritySink);
        }

        public void Push(LogEvent logEvent)
        {
            if(IsSinkEligible(logEvent) == false)
            {
                return;
            }
            HandleLogEvent(logEvent);
        }

        protected abstract void HandleLogEvent(LogEvent logEvent);

        protected abstract void PushToStore(IEnumerable<LogEvent> logBatch);
    }
}
