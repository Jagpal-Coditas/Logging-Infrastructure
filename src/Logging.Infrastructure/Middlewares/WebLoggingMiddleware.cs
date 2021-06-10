using Logging.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace Logging.Infrastructure.Middlewares
{
    public class WebLoggingMiddleware : BaseWebLoggingMiddleware
    {
        private readonly ILoggerContext _loggerContext;
        private readonly ILogger _logger;
        private readonly Func<Dictionary<string, Tuple<string, string>>> _getRouteMapper;

        public WebLoggingMiddleware(ILoggerContext loggerContext, ILogger logger, Func<Dictionary<string, Tuple<string, string>>> getRouteMapper)
            : base(loggerContext, logger)
        {
            _getRouteMapper = getRouteMapper;
        }

        protected override Task<ApiLog> GetLog(HttpRequestMessage request, HttpResponseMessage response)
        {
            _loggerContext.Set(LogContextProperty.Create(Constants.CORRELATIONID,
                LoggingHelper.GetHeaderIfPresent(request.Headers, Constants.HttpHeaders.CORRELATIONIDHEADER),
                true));
            _loggerContext.Set(LogContextProperty.Create(Constants.HOST, request.Headers.Host));
            _loggerContext.Set(LogContextProperty.Create(Constants.SCHEME, request.RequestUri.Scheme));
            _loggerContext.Set(LogContextProperty.Create(Constants.HTTPMETHOD, request.Method.Method));
            _loggerContext.Set(LogContextProperty.Create(Constants.CONTENTTYPE, LoggingHelper.GetHeaderIfPresent(request.Headers, Constants.HttpHeaders.CONTENTTYPEHEADER)));

            if (string.IsNullOrWhiteSpace(request.RequestUri.Query) == false)
            {
                _loggerContext.Set(LogContextProperty.Create(Constants.QUERYSTRING, request.RequestUri.Query));
            }

            return GetApiLog(request, response);
        }

        protected override bool ShouldLog(HttpRequestMessage request, HttpResponseMessage response)
        {
            return true;
        }

        private async Task<ApiLog> GetApiLog(HttpRequestMessage request, HttpResponseMessage response)
        {

        private void Log(HttpRequestMessage request, HttpResponseMessage response)
        {
           
        }

    }
}
