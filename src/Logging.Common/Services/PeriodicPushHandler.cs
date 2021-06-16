using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Logging.Common.Services
{
    public class PeriodicPushHandler : ILogEventPushHandler<IEnumerable<LogEvent>>
    {
        private const int DEFAULT_BLOCKING_COLLECTION_SIZE = int.MaxValue;
        private const int MAX_QUEUE_BLOCK_TIME_IN_MS = 100;
        private const int LOOP_TIME_IN_MS = 1000;

        private BlockingCollection<LogEvent> _queue;
        private bool _isTaskScheduled = false;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

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

        public bool AddOrPush(LogEvent logEvent, Func<IEnumerable<LogEvent>, bool> pushToStore)
        {
            var isSuccessfull = Queue.TryAdd(logEvent, MAX_QUEUE_BLOCK_TIME_IN_MS);

            if (isSuccessfull == false)
            {
                return HandleFailure(logEvent, pushToStore);
            }

            if (_isTaskScheduled == false)
            {
                SchedulePushTask(pushToStore);
            }
            return isSuccessfull;
        }

        private void SchedulePushTask(Func<IEnumerable<LogEvent>, bool> pushToStore)
        {
            Task.Run(async () => await PushQueueToStore(Queue, pushToStore, _cancellationTokenSource.Token), _cancellationTokenSource.Token);
            _isTaskScheduled = true;
        }

        private async Task PushQueueToStore(BlockingCollection<LogEvent> queue, Func<IEnumerable<LogEvent>, bool> pushToStore, CancellationToken cancellationToken)
        {
            while (true)
            {
                await Task.Delay(LOOP_TIME_IN_MS); // Internally handled by using Timer which is optimal than Thread.Sleep.
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var enumerable = queue.GetConsumingEnumerable();
                var logBatch = enumerable.ToList();
                pushToStore(logBatch);
            }
        }

        private bool HandleFailure(LogEvent logEvent, Func<IEnumerable<LogEvent>, bool> pushToStore)
        {
            // Retry push
            // return false if failed or return true
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel(); //Stopping the long running task when logging is terminated.
        }
    }
}
