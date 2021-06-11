using Logging.Common;
using Logging.Infrastructure.Helpers;
using System;
using System.Diagnostics;
using System.IO;
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

            return new Tuple<Message, Stopwatch>(request, stopWatch);
        }

        public void BeforeSendReply(ref Message response, object correlationState)
        {
            var correlationParams = correlationState as Tuple<Message, Stopwatch>;
            var request = correlationParams.Item1;
            var stopWatch = correlationParams.Item2;

            stopWatch.Stop();
            Log(request, response);
        }

        private void Log(Message request, Message response)
        {

            var requestContent = request.GetBody<string>();
            var responseContent = response.GetBody<string>();

            using (StreamWriter requestFile = new StreamWriter("Requests.txt"),
                responseFile = new StreamWriter("Response.txt"))
            {
                requestFile.WriteLine(requestContent);
                responseFile.WriteLine(responseContent);
            }

            // Log the request/response
        }
    }
}
