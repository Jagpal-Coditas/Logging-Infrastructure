﻿using Logging.Abstraction.Models;
using Logging.Abstraction.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Logging.Abstraction.Services
{
    public abstract class BasePeriodicPushSink : ISinkService
    {
        private const int DEFAULT_BLOCKING_COLLECTION_SIZE = int.MaxValue;
        private const int MAX_QUEUE_BLOCK_TIME_IN_MS = 100;
        private const int LOOP_TIME_IN_MS = 1000;

        private BlockingCollection<LogEvent> _queue;
        private bool _isTaskScheduled = false;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public BasePeriodicPushSink(ISinkService failOverSink, ILogEventFormatterService logEventFormatterService)
        {
            FailOverSink = failOverSink;
            LogFormatter = logEventFormatterService;
        }

        public ISinkService FailOverSink { get; }
        public ILogEventFormatterService LogFormatter { get; }

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
            var isSuccessfull = Queue.TryAdd(logEvent, MAX_QUEUE_BLOCK_TIME_IN_MS);

            if (isSuccessfull == false)
            {
                HandleFailure(logEvent);
            }

            if (_isTaskScheduled == false)
            {
                SchedulePushTask();
            }
        }

        protected abstract void PushToStore(IEnumerable<LogEvent> logBatch);

        private void HandleFailure(LogEvent logEvent)
        {
            //Step 1: Force push to Store
            PushQueueToStore();

            //Step 2 : Retry enqueueing the log
            var isSuccessfull = Queue.TryAdd(logEvent, MAX_QUEUE_BLOCK_TIME_IN_MS);

            //Step 3 : In case of failure add critical failure to fail over slink
            if (isSuccessfull == false)
            {
                FailOverSink.Send(logEvent);
                FailOverSink.Send(GetQueueStatsLog(Queue.Count, logEvent));
            }
        }

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
                logBatch.Add(Queue.Take());
            }

            try
            {
                PushToStore(logBatch);
            }
            catch (Exception e)
            {
                var queueInUse = Queue.Count / Queue.BoundedCapacity * 100;
                // add this to log
                // add exception to log
                var referenceLog = logBatch.First();

                LogEvent exceptionLog = GetExceptionLog(e, referenceLog);
                LogEvent queueStatsLog = GetQueueStatsLog(queueInUse, referenceLog);

                logBatch.Add(exceptionLog);
                logBatch.Add(queueStatsLog);

                foreach (var log in logBatch)
                {
                    FailOverSink.Send(log);
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