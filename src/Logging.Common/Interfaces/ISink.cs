using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logging.Common
{
    public interface ISink
    {
        bool IsFailOverSink { get; set; }
        bool Push(LogEvent logEvent);
        Task<bool> PushToStore(LogEvent logBatch);
        Task<bool> PushToStore(IEnumerable<LogEvent> logBatch);
    }
}
