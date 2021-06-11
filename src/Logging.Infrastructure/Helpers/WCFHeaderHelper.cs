using Logging.Common;
using System;
using System.ServiceModel.Channels;

namespace Logging.Infrastructure.Helpers
{
    public class WCFHeaderHelper
    {
        private static void ValidateHeaders(MessageHeaders headers)
        {
            if (headers == null || headers.Count < 1)
            {
                throw new LoggingMiddlewareException("Message headers not found");
            }
        }

        public static T GetHeaderIfPresent<T>(MessageHeaders headers, string ns, string headerName)
        {
            var index = headers.FindHeader(headerName, ns);
            if (index < 0)
            {
                return default(T);
            }
            return headers.GetHeader<T>(index);
        }

        public static string FetchNamespaceFromHeaders(MessageHeaders headers)
        {
            if (headers.Count < 1)
            {
                return null;
            }

            var firstHeader = headers[0];
            var ns = firstHeader.Namespace;
            return ns;
        }
    }
}
