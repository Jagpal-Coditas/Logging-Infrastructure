using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logging.Common.Models
{
    public class LoggerContext : ILoggerContext
    {
        private readonly ConcurrentDictionary<string, string> _loggingData = new ConcurrentDictionary<string, string>();
        
        public void Set(string key, string value)
        {
            _loggingData.TryAdd(key, value);
        }

        virtual public IDictionary<string, string> GetAll()
        {
            return _loggingData;
        }
    }
}
