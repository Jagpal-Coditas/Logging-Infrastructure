using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Infrastructure.Helpers
{
    public class HeaderHelper
    {
        public static string GetHeaderIfPresent(MessageHeaders headers, string ns, string headerName)
        {
            var index = headers.FindHeader(headerName, ns);
            if (index < 0)
            {
                return string.Empty;
            }
            return headers.GetHeader(index, ns);
        }
    }
}
