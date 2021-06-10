namespace Logging.Common.Models
{
    /// <summary>
    /// Common logging fields to log an API endpoint communication. Indicating developers to log these fields.
    /// </summary>
    public class ApiLog : Log
    {
        public string Api { get; set; }
        public string Verb { get ; set; }
        public bool IsSuccessful { get ; set; }
        public byte[] Request { get; set; } //new Payload(LoggingMiddlewareUtility.GetResponsePayloadBytes(httpContext))
        public byte[] Response { get; set; }
        public string Url { get; set; }
        public long TimeTakenInms { get; set; }
    }
}
