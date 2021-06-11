using System;

namespace Logging.Common
{
    public class LoggingMiddlewareException : Exception
    {
        public LoggingMiddlewareException()
        {
        }

        public LoggingMiddlewareException(string message) : base(message)
        {
        }

        public LoggingMiddlewareException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
