using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logging.Common.Services
{
    public interface ILogEventPushHandler
    {
        bool AddOrPush(LogEvent logEvent, Func<LogEvent, bool> pushToStore, Func<IEnumerable<LogEvent>, bool> bulkPushToStore);
    }
}