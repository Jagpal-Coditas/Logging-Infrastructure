using Logging.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
namespace Logging.Infrastructure.Middlewares
{
    public class WebLoggingMiddleware : BaseWebLoggingMiddleware
    {
        private readonly Func<HttpRequestMessage, Tuple<string, string>> _getApiAndVerb;
        private Action<ILoggerContext, HttpRequestMessage> _enrichLoggerContext;
        //public WebLoggingMiddleware(ILoggerContext loggerContext,
        //    ILogger logger,
        //    Func<HttpRequestMessage, Tuple<string, string>> getApiAndVerb)
        //    : base(loggerContext, logger)
        //{
        //    _getApiAndVerb = getApiAndVerb;
        //}

        //public WebLoggingMiddleware(ILoggerContext loggerContext,
        //    ILogger logger,
        //    Func<HttpRequestMessage, Tuple<string, string>> getApiAndVerb,
        //    Action<ILoggerContext, HttpRequestMessage> enrichLoggerContext)
        //    : base(loggerContext, logger)
        //{
        //    _getApiAndVerb = getApiAndVerb;
        //    _enrichLoggerContext = enrichLoggerContext;
        //}
        public WebLoggingMiddleware(ILoggerContext loggerContext, ILogger logger)
            : base(loggerContext, logger)
        {
        }

        protected override async Task<ApiLog> GetLog(HttpRequestMessage request, HttpResponseMessage response)
        {
            LoggerContext.Set(LogContextProperty.Create(Constants.CORRELATIONID,
                LoggingHelper.GetHeaderIfPresent(request.Headers, Constants.HttpHeaders.CORRELATIONIDHEADER),
                true));
            LoggerContext.Set(LogContextProperty.Create(Constants.HOST, request.Headers.Host));
            LoggerContext.Set(LogContextProperty.Create(Constants.SCHEME, request.RequestUri.Scheme));
            LoggerContext.Set(LogContextProperty.Create(Constants.HTTPMETHOD, request.Method.Method));
            LoggerContext.Set(LogContextProperty.Create(Constants.CONTENTTYPE, LoggingHelper.GetHeaderIfPresent(request.Headers, Constants.HttpHeaders.CONTENTTYPEHEADER)));

            if (string.IsNullOrWhiteSpace(request.RequestUri.Query) == false)
            {
                LoggerContext.Set(LogContextProperty.Create(Constants.QUERYSTRING, request.RequestUri.Query));
            }
            if (_enrichLoggerContext != null)
            {
                _enrichLoggerContext(LoggerContext, request);
            }
            return await GetApiLog(request, response);
        }

        protected override bool ShouldLog(HttpRequestMessage request, HttpResponseMessage response)
        {
            return true;
        }

        private async Task<ApiLog> GetApiLog(HttpRequestMessage request, HttpResponseMessage response)
        {
            //var apiVerb = _getApiAndVerb(request);
            var apiLog = new ApiLog()
            {
                Api = "apiVerb.Item1",
                Verb = "apiVerb.Item2",
                IsSuccessful = response.StatusCode == System.Net.HttpStatusCode.OK,
                Url = request.RequestUri.OriginalString,
                Request = await GetRequestPayload(request),
                Response = await GetResponsePayload(response)
            };

            return apiLog;
        }

    }
}
