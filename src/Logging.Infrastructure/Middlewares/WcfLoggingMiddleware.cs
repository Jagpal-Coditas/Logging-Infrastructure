using Logging.Common;
using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Logging.Infrastructure.Middlewares
{
    public class WcfLoggingMiddleware : IDispatchMessageInspector
    {
        // TODO : MSMQ logging not handled
        // TODO : Namespace handling verification required

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var firstHeader = request.Headers[0];
            var ns = firstHeader.Namespace;

            string correlationId = GetCorrelationIdFromHeader(request, ns);
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
                MessageHeader correlationHeader = GetCorrelationHeader(ns, correlationId);
                request.Headers.Add(correlationHeader);
            }

            return new Tuple<Message, Stopwatch>(request, stopWatch);
        }

        public void BeforeSendReply(ref Message response, object correlationState)
        {
            var correlationParams = correlationState as Tuple<Message, Stopwatch>;
            var request = correlationParams.Item1;
            var stopWatch = correlationParams.Item2;
            var ns = string.Empty;

            stopWatch.Stop();
            Log(request, response);
            if (string.IsNullOrEmpty(GetCorrelationIdFromHeader(response, ns)))
                response.Headers.Add(GetCorrelationHeader(ns, Guid.NewGuid().ToString()));
        }

        private void Log(Message request, Message response)
        {
            // Log the request/response
        }

        private string GetCorrelationIdFromHeader(Message request, string ns)
        {
            return string.Empty;
            //return HeaderHelper.GetHeaderIfPresent(request.Headers, ns, Constants.HttpHeaders.CORRELATIONIDHEADER);
        }

        private MessageHeader GetCorrelationHeader(string ns, string correlationId)
        {
            return MessageHeader.CreateHeader(Constants.HttpHeaders.CORRELATIONIDHEADER, ns, correlationId);
        }
    }
}
