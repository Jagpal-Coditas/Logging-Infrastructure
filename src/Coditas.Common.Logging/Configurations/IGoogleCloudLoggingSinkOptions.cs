using System.Collections.Generic;

namespace Coditas.Common.Logging.Configurations
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
