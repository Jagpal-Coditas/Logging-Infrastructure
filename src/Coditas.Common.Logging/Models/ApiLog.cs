using System.Collections.Generic;

namespace Coditas.Common.Logging.Models
{
    /// <summary>
    /// Common logging fields to log an API endpoint communication. Indicating developers to log these fields.
    /// </summary>
    public class ApiLog : LogEvent
    {
        public ApiLog(string appName, string env, string api, string verb) : base(appName, env)
        {
            Api = api;
            Verb = verb;
            RequestHeaders = new Dictionary<string, string>();
            ResponseHeaders = new Dictionary<string, string>();
        }

        public IDictionary<string, string> RequestHeaders { get; }
        public IDictionary<string, string> ResponseHeaders { get; }
        public string Api { get; set; }
        public string Verb { get; set; }
        public bool IsSuccessful { get; set; }
        public byte[] Request { get; set; }
        public byte[] Response { get; set; }
        public string Url { get; set; }
        public long TimeTakenInms { get; set; }
    }
}
