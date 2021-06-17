using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Logging.Common.Services
{
    public abstract class BasePeriodicPushSink : BaseSink, ISink
    {
        private readonly ISink _innerSink;

        private const int DEFAULT_BLOCKING_COLLECTION_SIZE = int.MaxValue;
        private const int MAX_QUEUE_BLOCK_TIME_IN_MS = 100;
        private const int LOOP_TIME_IN_MS = 1000;

        private BlockingCollection<LogEvent> _queue;
        private bool _isTaskScheduled = false;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        //public BasePeriodicPushSink(ISink innerSink)
        //{
        //    _innerSink = innerSink;
        //}

        public BasePeriodicPushSink()
        {
            _innerSink = null;
        }

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

        public override bool IsFailOverSink //TODO : remove
        {
            get
            {
                return true;
            }
        }

        public override bool IsPrioritySink
        {
            get
            {
                return false;
            }
        }

        protected override void HandleLogEvent(LogEvent logEvent)
        {
            var isSuccessfull = Queue.TryAdd(logEvent, MAX_QUEUE_BLOCK_TIME_IN_MS);

            if (isSuccessfull == false)
            {
                HandleFailure(logEvent);
            }
        }

        private void HandleFailure(LogEvent logEvent)
        {
            //Step 1: Force push to Store
            PushQueueToStore(Queue, PushToStore, _innerSink);

            //Step 2 : Retry enqueueing the log
            var isSuccessfull = Queue.TryAdd(logEvent, MAX_QUEUE_BLOCK_TIME_IN_MS);

            //Step 3 : In case of failure add critical failure to fail over slink
            if (isSuccessfull == false)
            {
                _innerSink.Push(logEvent);
                _innerSink.Push(GetQueueStatsLog(Queue.Count, logEvent));
            }

            if (_isTaskScheduled == false)
            {
                SchedulePushTask();
            }
        }

        private void SchedulePushTask()
        {
            Task.Run(async () => await PushQueueToStoreAsync(Queue, PushToStore, _innerSink, _cancellationTokenSource.Token), _cancellationTokenSource.Token);
            _isTaskScheduled = true;
        }

        private async Task PushQueueToStoreAsync(BlockingCollection<LogEvent> queue, Action<IEnumerable<LogEvent>> pushToStore, ISink _innerSink, CancellationToken cancellationToken)
        {
            while (true)
            {
                await Task.Delay(LOOP_TIME_IN_MS); // Internally handled by using Timer which is optimal than Thread.Sleep.
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                PushQueueToStore(queue, pushToStore, _innerSink);
            }
        }

        private void PushQueueToStore(BlockingCollection<LogEvent> queue, Action<IEnumerable<LogEvent>> pushToStore, ISink _innerSink)
        {
            var enumerable = queue.GetConsumingEnumerable();
            var logBatch = enumerable.ToList();
            if (logBatch.Count < 1)
            {
                return;
            }

            try
            {
                pushToStore(logBatch);
            }
            catch (Exception e)
            {
                var queueInUse = queue.Count / queue.BoundedCapacity * 100;
                // add this to log
                // add exception to log
                var referenceLog = logBatch.First();

                LogEvent exceptionLog = GetExceptionLog(e, referenceLog);
                LogEvent queueStatsLog = GetQueueStatsLog(queueInUse, referenceLog);

                logBatch.Add(exceptionLog);
                logBatch.Add(queueStatsLog);

                foreach (var log in logBatch)
                {
                    _innerSink.Push(log);
                }
            }
        }

        private static LogEvent GetQueueStatsLog(int queueInUse, LogEvent referenceLog)
        {
            return LogEvent.Create(referenceLog.AppName, referenceLog.Environment, LogLevel.Critical, string.Format("Queue stats : Occupied capacity {0}%", queueInUse));
            // add more stats to understand system status.
        }

        private static LogEvent GetExceptionLog(Exception e, LogEvent referenceLog)
        {
            return LogEvent.Create(referenceLog.AppName, referenceLog.Environment, LogLevel.Critical, e.Message + e.StackTrace);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel(); //Stopping the long running task when logging is terminated.
        }
    }
}
