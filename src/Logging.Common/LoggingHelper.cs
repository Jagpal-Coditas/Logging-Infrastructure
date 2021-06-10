using System.Linq;
using System.Net.Http.Headers;

namespace Logging.Common
{
    public static class LoggingHelper
    {
        public static string GetHeaderIfPresent(HttpRequestHeaders headers, string headerName)
        {
            if (headers.Contains(headerName))
            {
                return headers.GetValues(headerName).First();
            }
            return string.Empty;
        }
    }

}
