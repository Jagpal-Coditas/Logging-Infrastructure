using Logging.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Logging.Infrastructure.Middlewares
{
    public class CorrelationIdHandler : DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var correlationId = LoggingHelper.GetHeaderIfPresent(request.Headers, Constants.HttpHeaders.CORRELATIONIDHEADER);
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
                request.Headers.Add(Constants.HttpHeaders.CORRELATIONIDHEADER, correlationId);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.Headers.Contains(Constants.HttpHeaders.CORRELATIONIDHEADER) == false)
                response.Headers.Add(Constants.HttpHeaders.CORRELATIONIDHEADER, correlationId);

            return response;
        }
    }
}
