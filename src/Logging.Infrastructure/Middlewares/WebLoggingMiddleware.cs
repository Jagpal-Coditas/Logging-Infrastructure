using Logging.Common;
using Logging.Common.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace Logging.Infrastructure.Middlewares
{
    public class WebLoggingMiddleware : DelegatingHandler
    {
        private readonly ILoggerContext _loggerContext;
        private readonly ILogger _logger;
        private readonly Func<Dictionary<string, Tuple<string, string>>> _getRouteMapper;

        public WebLoggingMiddleware(ILoggerContext loggerContext, ILogger logger, Func<Dictionary<string, Tuple<string, string>>> getRouteMapper)
        {
            if (loggerContext == null)
                throw new ArgumentNullException(typeof(ILoggerContext).FullName);

            if (logger == null)
                throw new ArgumentNullException(typeof(ILogger).FullName);

            _loggerContext = loggerContext;
            _logger = logger;
            _getRouteMapper = getRouteMapper;
        }
        protected async override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var correlationId = LoggingHelper.GetHeaderIfPresent(request.Headers, Constants.HttpHeaders.CORRELATIONIDHEADER);
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
                request.Headers.Add(Constants.HttpHeaders.CORRELATIONIDHEADER, correlationId);
            }

            _loggerContext.Set(LogContextProperty.Create(Constants.CORRELATIONID, correlationId, true));
            _loggerContext.Set(LogContextProperty.Create(Constants.HOST, request.Headers.Host));
            _loggerContext.Set(LogContextProperty.Create(Constants.SCHEME, request.RequestUri.Scheme));
            _loggerContext.Set(LogContextProperty.Create(Constants.HTTPMETHOD, request.Method.Method));
            _loggerContext.Set(LogContextProperty.Create(Constants.CONTENTTYPE, LoggingHelper.GetHeaderIfPresent(request.Headers, Constants.HttpHeaders.CONTENTTYPEHEADER)));

            if (string.IsNullOrWhiteSpace(request.RequestUri.Query) == false)
            {
                _loggerContext.Set(LogContextProperty.Create(Constants.QUERYSTRING, request.RequestUri.Query));
            }

            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);

            try
            {
                response = await base.SendAsync(request, cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(new EventId(), exception, exception.Message);
            }
            finally
            {
                stopWatch.Stop();
                var apiLog = await GetApiLog(request, response, stopWatch);
                _logger.LogInformation("Request Executed", apiLog);
                if (response.Headers.Contains(Constants.HttpHeaders.CORRELATIONIDHEADER) == false)
                    response.Headers.Add(Constants.HttpHeaders.CORRELATIONIDHEADER, correlationId);

            }
            return response;
        }

        private async Task<ApiLog> GetApiLog(HttpRequestMessage request, HttpResponseMessage response, Stopwatch stopWatch)
        {
            if (stopWatch.IsRunning)
                stopWatch.Stop();

            var apiLog = new ApiLog()
            {
                Api = "",
                Verb = "",
                ApplicationName = "",
                ClientIp = "",
                CorrelationId = "",
                IsSuccessful = response.StatusCode == System.Net.HttpStatusCode.OK,
                Url = "",
                LogTime = DateTime.UtcNow,
                TimeTakenInms = stopWatch.ElapsedMilliseconds
            };
            var requestData = await request.Content.ReadAsByteArrayAsync();

            var responseData = await response.Content.ReadAsByteArrayAsync();

            if (requestData.Length > 0)
                apiLog.Request = requestData;
            if (responseData.Length > 0)
                apiLog.Response = responseData;
            return apiLog;
        }

    }
}
