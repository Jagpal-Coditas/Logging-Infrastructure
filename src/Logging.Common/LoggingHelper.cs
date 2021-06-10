using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Logging.Common
{
    public static class LoggingHelper
    {
        public static async void EnrichFromRequest(ILoggerContext loggerContext, HttpRequestMessage request)
        {
            var correlationId = GetHeaderIfPresent(request.Headers, Constants.HttpHeaders.CORRELATIONIDHEADER);

            // Set all the common properties available for every request
            loggerContext.Set(Constants.CORRELATIONID, correlationId);
            loggerContext.Set(Constants.HOST, request.Headers.Host);
            loggerContext.Set(Constants.SCHEME, request.RequestUri.Scheme);
            loggerContext.Set(Constants.HTTPMETHOD, request.Method.Method);
            loggerContext.Set(Constants.CONTENTTYPE, GetHeaderIfPresent(request.Headers, Constants.HttpHeaders.CONTENTTYPEHEADER));
            // Only set it if available. You're not sending sensitive data in a querystring right?!
            if (string.IsNullOrWhiteSpace(request.RequestUri.Query) == false)
            {
                loggerContext.Set(Constants.QUERYSTRING, request.RequestUri.Query);
            }
        }
        public static string GetHeaderIfPresent(HttpRequestHeaders headers, string headerName)
        {
            if (headers.Contains(headerName))
            {
                return headers.GetValues(headerName).First();
            }
            return string.Empty;
        }
    }

}
