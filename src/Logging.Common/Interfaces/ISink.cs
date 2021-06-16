using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logging.Common
{
    public interface ISink<T>
    {
        bool IsFailOverSink { get; set; }
        bool Push(LogEvent logEvent);
        Task<bool> PushToStore(T logBatch);
    }
}
