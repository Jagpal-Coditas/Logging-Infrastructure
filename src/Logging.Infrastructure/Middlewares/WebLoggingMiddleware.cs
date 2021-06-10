using Logging.Common;
using Logging.Common.Models;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace Logging.Infrastructure.Middlewares
{
    public class WebLoggingMiddleware : DelegatingHandler
    {
        private readonly ILoggerContext _loggerContext;
        public WebLoggingMiddleware(ILoggerContext loggerContext)
        {
            if (loggerContext == null)
                throw new ArgumentNullException(typeof(ILoggerContext).FullName);
            _loggerContext = loggerContext;
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
            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            
            try
            {
                response = await base.SendAsync(request, cancellationToken);
            }
            catch (Exception exception)
            {

            }
            finally
            {
                LoggingHelper.EnrichFromRequest(_loggerContext, request);
                stopWatch.Stop();
                Log(request, response);
                if (response.Headers.Contains(Constants.HttpHeaders.CORRELATIONIDHEADER) == false)
                    response.Headers.Add(Constants.HttpHeaders.CORRELATIONIDHEADER, correlationId);
            }
            return response;
        }

        private void Log(HttpRequestMessage request, HttpResponseMessage response)
        {
        }

    }
}
