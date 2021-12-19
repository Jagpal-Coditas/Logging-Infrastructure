using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Coditas.Common.Logging.Services
{
    public class LoggerContextService : ILoggerContextService
    {
        private readonly ConcurrentDictionary<string, object> _loggingData = new ConcurrentDictionary<string, object>();

        public void Set(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key) && value == null)
                return;
            var fields = Utility.GetFormattedKeyValues(key, value);
            foreach (var field in fields)
            {
                _loggingData.AddOrUpdate(field.Key, field.Value, (currentKey, currentValue) => field.Value);
            }
        }

        public IReadOnlyDictionary<string, object> Get()
        {
            return (IReadOnlyDictionary<string, object>)_loggingData;
        }

    }
}
