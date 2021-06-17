//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Logging.Common.Services
//{
//    public class BufferedQuickPushHandler
//    {
//        private const int DEFAULT_BLOCKING_COLLECTION_SIZE = int.MaxValue;
//        private const int MAX_QUEUE_BLOCK_TIME_IN_MS = 100;

//        private BlockingCollection<LogEvent> _queue;

//        public virtual BlockingCollection<LogEvent> BufferQueue
//        {
//            get
//            {
//                if (_queue == null)
//                {
//                    _queue = new BlockingCollection<LogEvent>(DEFAULT_BLOCKING_COLLECTION_SIZE); // Default blocking collection - ConcurrentQueue with default MaxBound.
//                }
//                return _queue;
//            }
//        }

//        public bool AddOrPush(LogEvent logEvent, Func<LogEvent, bool> pushToStore)
//        {
//            //critical
//            var isSuccessfull = BufferQueue.TryAdd(logEvent, MAX_QUEUE_BLOCK_TIME_IN_MS);

//            if (isSuccessfull == false)
//            {
//                return HandleFailure(logEvent, pushToStore);
//            }
//            PushQueueToStore();

//            return isSuccessfull;
//        }

//        private Task<bool> PushQueueToStore(Func<LogEvent, Task<bool>> pushToStore)
//        {
//            var enumerable = BufferQueue.GetConsumingEnumerable();
//            var logBatch = enumerable.ToList();
//            return pushToStore(logBatch);
//        }

//        private bool HandleFailure(LogEvent logEvent, Func<LogEvent, bool> pushToStore)
//        {
//            // Retry push
//            // return false if failed or return true
//            throw new NotImplementedException();
//        }

//        public void Dispose()
//        {
//            _cancellationTokenSource.Cancel(); //Stopping the long running task when logging is terminated.
//        }
//    }
//}
