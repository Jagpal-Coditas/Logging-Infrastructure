using System;

namespace Logging.Common.Models
{
    /// <summary>
    /// Necessary logging fields across all applications.
    /// </summary>
    public class Log
    {
        public string CorrelationId { get; set; }
        public string ApplicationName { get; set; }
        public DateTime LogTime { get; set; }
        public string Message { get; set; }
        public string ClientIp { get; set; }
        public string TransactionId { get; set; } // Transaction id for tracking transactions currently used by business
    }
}