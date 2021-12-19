using Coditas.Common.Logging.Models;
using System.Collections.Generic;

namespace Coditas.Common.Logging.Services
{
    public interface ISinkService
    {
        string Name { get; }
        IEnumerable<ILogFormatter> LogFormatters { get; }
        void Send(LogEvent logEvent);
    }
}
