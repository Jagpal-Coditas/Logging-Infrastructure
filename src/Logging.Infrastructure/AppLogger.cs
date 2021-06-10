using Microsoft.Extensions.Logging;

namespace Logging.Infrastructure
{
    public static class AppLogger
    {
        private static ILoggerFactory _loggerFactory;
        public static void Init(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }
        public static ILogger CreateLogger<T>()
        {

            if (_loggerFactory == null)
                return null;

            return _loggerFactory.CreateLogger<T>();
        }
        public static ILogger CreateLogger(string categoryName)
        {
            if (_loggerFactory == null)
                return null;

            return _loggerFactory.CreateLogger(categoryName);
        }
    }
}
