using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;

namespace Logging.Web
{
    
    internal class ContextHelper
    {
        // Mahesh :  httprequest headers and response headers are already dictionary so can be directly added to log.
        public Task<string> GetRequest(HttpRequestMessage httpRequestMessage)
        {
            return httpRequestMessage == null ? Task.FromResult(string.Empty) : httpRequestMessage.Content?.ReadAsStringAsync();
        }

        public Task<string> GetResponse(HttpRequestMessage httpResponseMessage)
        {
            return httpResponseMessage == null ? Task.FromResult(string.Empty) : httpResponseMessage.Content?.ReadAsStringAsync();
        }

        /// <summary>
        /// Fill all necessary data in the 
        /// </summary>
        public Dictionary<string, string> GetMandatoryFields()
        {
            IsSuccessful = httpContext.Response?.StatusCode == 200,
             public string ApplicationName { get; set; }
            public string Api { get; set; }
            public string Verb { get; set; }
            public string IsSuccessful { get; set; }
            public string Request { get; set; } //new Payload(LoggingMiddlewareUtility.GetResponsePayloadBytes(httpContext))
            public string Response { get; set; }
            public string Url { get; set; }
            public string ClientIp { get; set; } // Mahesh : required?
            public string TransactionId { get; set; } // Transaction id for tracking transactions currently used by business
    }
    }
}
