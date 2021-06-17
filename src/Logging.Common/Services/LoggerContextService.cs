using Logging.Abstraction.Models;
using Logging.Abstraction.Services;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Logging.Common.Services
{
    public class LoggerContextService : ILoggerContextService
    {
        private readonly ConcurrentBag<LogContextProperty> _loggingData = new ConcurrentBag<LogContextProperty>();

        public void Set(LogContextProperty prop)
        {
            _loggingData.Add(prop);
        }

        public IEnumerable<LogContextProperty> Get(bool IsMiddlewareLogging = false)
        {
            if (IsMiddlewareLogging)
                return _loggingData;

            return _loggingData.Where(prop => prop.IsCommonLog);
        }

    }
}
