using Coditas.Common.Logging.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Coditas.Common.Web.Middlewares
{
    public abstract class BaseWebLoggingMiddleware : DelegatingHandler
    {
        private readonly ILogger _logger;
        public BaseWebLoggingMiddleware(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(typeof(ILogger).FullName);

            _logger = logger;
        }
        protected async override Task<HttpResponseMessage> SendAsync(
           HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            bool shouldLog = false;
            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);

            try
            {
                shouldLog = ShouldLog(request, response);
                response = await base.SendAsync(request, cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(new EventId(), exception, exception.Message);
            }
            finally
            {
                if (shouldLog)
                    Log(request, response, stopWatch);
            }
            return response;
        }

        async Task Log(HttpRequestMessage request, HttpResponseMessage response, Stopwatch stopWatch)
        {
            if (stopWatch.IsRunning)
                stopWatch.Stop();

            var apiLogEntry = await GetLog(request, response);
            apiLogEntry.TimeTakenInms = stopWatch.ElapsedMilliseconds;

            _logger.LogInformation("Request Executed", apiLogEntry);
        }

        protected abstract Task<ApiLog> GetLog(HttpRequestMessage request, HttpResponseMessage response);

        protected abstract bool ShouldLog(HttpRequestMessage request, HttpResponseMessage response);

        protected internal virtual async Task<byte[]> GetRequestPayload(HttpRequestMessage request)
        {
            var requestData = await request.Content.ReadAsByteArrayAsync();

            if (requestData.Length > 0)
                return requestData;

            return new byte[0];
        }
        protected internal virtual async Task<byte[]> GetResponsePayload(HttpResponseMessage response)
        {
            var responseData = await response.Content.ReadAsByteArrayAsync();

            if (responseData.Length > 0)
                return responseData;

            return new byte[0];
        }
    }
}
