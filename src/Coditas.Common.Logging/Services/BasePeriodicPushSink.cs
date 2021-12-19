using Coditas.Common.Logging.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Coditas.Common.Logging.Services
{
    public abstract class BasePeriodicPushSink : ISinkService
    {
        private const int DEFAULT_BLOCKING_COLLECTION_SIZE = int.MaxValue;
        private const int MAX_QUEUE_BLOCK_TIME_IN_MS = 100;
        private const int LOOP_TIME_IN_MS = 1000;

        private BlockingCollection<LogEvent> _queue;
        private bool _isTaskScheduled = false;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public BasePeriodicPushSink(string name, IEnumerable<ILogFormatter> logEventFormatters = null)
        {
            Name = name;
            LogFormatters = logEventFormatters;
        }
        public string Name { get; }
        public IEnumerable<ILogFormatter> LogFormatters { get; }

        public virtual BlockingCollection<LogEvent> Queue
        {
            get
            {
                if (_queue == null)
                {
                    _queue = new BlockingCollection<LogEvent>(DEFAULT_BLOCKING_COLLECTION_SIZE); // Default blocking collection - ConcurrentQueue with default MaxBound.
                }
                return _queue;
            }
        }

        public void Send(LogEvent logEvent)
        {
            Queue.TryAdd(logEvent, MAX_QUEUE_BLOCK_TIME_IN_MS);

            if (_isTaskScheduled == false)
            {
                SchedulePushTask();
            }
        }

        protected abstract void PushToStore(IEnumerable<LogEvent> logBatch);

        private void SchedulePushTask()
        {
            Task.Run(async () => await PushQueueToStoreAsync(), _cancellationTokenSource.Token);
            _isTaskScheduled = true;
        }

        private async Task PushQueueToStoreAsync()
        {
            while (true)
            {
                await Task.Delay(LOOP_TIME_IN_MS); // Internally handled by using Timer which is optimal than Thread.Sleep.
                if (_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    return;
                }
                PushQueueToStore();
            }
        }

        private void PushQueueToStore()
        {
            if (Queue.Count < 1)
            {
                return;
            }

            var logBatch = new List<LogEvent>();
            while (Queue.Count != 0)
            {
                var logEvent = Queue.Take();
                if (LogFormatters != null && LogFormatters.Any())
                {
                    foreach (var formatter in LogFormatters)
                    {
                        logEvent = formatter.Format(logEvent);
                    }
                }

                logBatch.Add(logEvent);
            }

            PushToStore(logBatch);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel(); //Stopping the long running task when logging is terminated.
        }
    }
}
