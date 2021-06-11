using Logging.Common;
using Logging.Infrastructure.Helpers;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Logging.Infrastructure.Middlewares
{
    public class WCFCorrelationHandler : IDispatchMessageInspector
    {
        // TODO : Namespace handling verification required

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var ns = WCFHeaderHelper.FetchNamespaceFromHeaders(request.Headers);
            var correlationId = GetCorrelationIdFromHeader(request.Headers, ns);

            if (string.IsNullOrWhiteSpace(correlationId))
            {
                var correlationHeader = CreateCorrelationHeader(ns);
                request.Headers.Add(correlationHeader);
            }
            return null; // Correlation state is not required
        }

        public void BeforeSendReply(ref Message response, object correlationState)
        {
            var ns = WCFHeaderHelper.FetchNamespaceFromHeaders(response.Headers);
            var correlationId = GetCorrelationIdFromHeader(response.Headers, ns);

            if (string.IsNullOrWhiteSpace(correlationId))
                response.Headers.Add(CreateCorrelationHeader(ns));
        }

        private string GetCorrelationIdFromHeader(MessageHeaders headers, string ns)
        {
            return WCFHeaderHelper.GetHeaderIfPresent<string>(headers, ns, Constants.HttpHeaders.CORRELATIONIDHEADER);
        }

        private MessageHeader CreateCorrelationHeader(string ns)
        {
            return MessageHeader.CreateHeader(Constants.HttpHeaders.CORRELATIONIDHEADER, ns, Guid.NewGuid().ToString());
        }
    }
}
