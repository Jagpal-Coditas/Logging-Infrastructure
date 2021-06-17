using Logging.Abstraction.Configuration;
using System.Collections.Generic;

namespace Logging.Common.Configurations
{
    public class GoogleCloudLoggingSinkOptions : IGoogleCloudLoggingSinkOptions
    {
        public const string GcpLoggingOptions = "gcplogging";
        public string ProjectId { get; set; }

        public string ResourceType { get; set; }

        public string LogName { get; set; }

        public Dictionary<string, string> Labels { get; }

        public Dictionary<string, string> ResourceLabels { get; }

        public bool UseLoggerContextAsLogName { get; set; }

        /// <summary>
        /// Integrate logs with Cloud Trace by setting LogEntry.Trace and LogEntry.SpanId if the LogEvent contains TraceId and SpanId properties.
        /// Required for Google Cloud Trace Log Correlation.
        /// See https://cloud.google.com/trace/docs/trace-log-integration
        /// </summary>
        public bool UseLogCorrelation { get; set; }

        /// <summary>
        /// JSON string of Google Cloud credentials file, otherwise will use Application Default credentials found on host by default.
        /// </summary>
        public string GoogleCredentialJson { get; set; }
        public GoogleCloudLoggingSinkOptions(
            string projectId,
            string resourceType = null,
            string logName = null,
            Dictionary<string, string> labels = null,
            Dictionary<string, string> resourceLabels = null,
            bool useLoggerContextAsLogName = true,
            bool useLogCorrelation = true,
            string googleCredentialJson = null)
        {
            ProjectId = projectId;
            ResourceType = resourceType;
            LogName = logName ?? "Default";
            Labels = new Dictionary<string, string>();
            ResourceLabels = new Dictionary<string, string>();

            if (labels != null)
                foreach (var kvp in labels)
                    Labels[kvp.Key] = kvp.Value;

            if (resourceLabels != null)
                foreach (var kvp in resourceLabels)
                    ResourceLabels[kvp.Key] = kvp.Value;

            UseLoggerContextAsLogName = useLoggerContextAsLogName;
            UseLogCorrelation = useLogCorrelation;
            GoogleCredentialJson = googleCredentialJson;
        }
    }
}
