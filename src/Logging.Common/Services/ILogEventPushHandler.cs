using System;
using System.Threading.Tasks;

namespace Logging.Common.Services
{
    public interface ILogEventPushHandler<T>
    {
        bool AddOrPush(LogEvent logEvent, Func<T, bool> pushToStore);
    }
}