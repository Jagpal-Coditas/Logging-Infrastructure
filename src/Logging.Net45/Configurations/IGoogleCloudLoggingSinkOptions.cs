using System.Collections.Generic;

namespace Logging.Net45
{
    public interface IGoogleCloudLoggingSinkOptions
    {
        string ProjectId { get; set; }

        string ResourceType { get; set; }

        string LogName { get; set; }

        Dictionary<string, string> Labels { get; }

        Dictionary<string, string> ResourceLabels { get; }

        bool UseLoggerContextAsLogName { get; set; }

        bool UseLogCorrelation { get; set; }

        string GoogleCredentialJson { get; set; }
    }
}
