using Google.Api;
using Google.Api.Gax.Grpc;
using Google.Cloud.Logging.Type;
using Google.Cloud.Logging.V2;
using Google.Protobuf.WellKnownTypes;
using Logging.Common;
using Logging.Common.Services;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MsLogging = Microsoft.Extensions.Logging;
namespace Logging.Net45
{
    public class GoogleCloudLoggingSink : BasePeriodicPushSink
    {
        private readonly IGoogleCloudLoggingSinkOptions _loggingSinkOptions;
        private readonly LoggingServiceV2Client _client;
        private readonly MonitoredResource _resource;
        private readonly string _projectId;
        private string _logName;
        private static readonly Dictionary<string, string> LogNameCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static readonly Regex LogNameUnsafeChars = new Regex("[^0-9A-Z._/-]+", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant);
        public GoogleCloudLoggingSink(IGoogleCloudLoggingSinkOptions loggingSinkOptions, ILogEventFormatter logEventFormatter)
        {
            if (loggingSinkOptions == null)
                throw new ArgumentNullException(typeof(IGoogleCloudLoggingSinkOptions).FullName);

            LogFormatter = logEventFormatter;
            _loggingSinkOptions = loggingSinkOptions;

            _resource = MonitoredResourceBuilder.FromPlatform();
            _resource.Type = _loggingSinkOptions.ResourceType ?? _resource.Type;
            foreach (var kvp in _loggingSinkOptions.ResourceLabels)
                _resource.Labels[kvp.Key] = kvp.Value;

            _projectId = _loggingSinkOptions.ProjectId ?? (_resource.Labels.TryGetValue("project_id", out string id) ? id : null) ?? "";
            if (String.IsNullOrWhiteSpace(_projectId))
                throw new ArgumentNullException(typeof(IGoogleCloudLoggingSinkOptions).FullName + ".ProjectId", "Project Id is not provided and could not be automatically discovered.");

            if (String.IsNullOrWhiteSpace(_loggingSinkOptions.LogName))
                throw new ArgumentNullException(typeof(IGoogleCloudLoggingSinkOptions).FullName + ".LogName", "Log Name is blank. Check assigned value or unset to use default.");

            _logName = CreateLogName(_projectId, _loggingSinkOptions.LogName);

            _client = _loggingSinkOptions.GoogleCredentialJson != null
                ? new LoggingServiceV2ClientBuilder { JsonCredentials = _loggingSinkOptions.GoogleCredentialJson }.Build()
                : LoggingServiceV2Client.Create();
        }

        public override ILogEventFormatter LogFormatter { get; }

        public bool IsPrioritySink { get { return false; } }

        public bool IsFailOverSink { get { return true; } }

        public void Send(LogEvent logEvent)
        {
            if (LogFormatter != null)
                logEvent = LogFormatter.Format(logEvent);
            WriteLogEntry(logEvent);
        }
        private static LogSeverity TranslateSeverity(MsLogging.LogLevel level)
        {
            LogSeverity logSeverity;
            switch (level)
            {
                case MsLogging.LogLevel.Trace:
                case MsLogging.LogLevel.Debug:
                    logSeverity = LogSeverity.Debug;
                    break;
                case MsLogging.LogLevel.Information:
                    logSeverity = LogSeverity.Info;
                    break;
                case MsLogging.LogLevel.Warning:
                    logSeverity = LogSeverity.Warning;
                    break;
                case MsLogging.LogLevel.Error:
                    logSeverity = LogSeverity.Error;
                    break;
                case MsLogging.LogLevel.Critical:
                    logSeverity = LogSeverity.Critical;
                    break;
                default:
                    logSeverity = LogSeverity.Default;
                    break;
            }
            return logSeverity;
        }

        private void WriteLogEntry(LogEvent logEvent)
        {
            if (_loggingSinkOptions.UseLoggerContextAsLogName && string.IsNullOrWhiteSpace(logEvent.SourceContext) == false)
                _logName = CreateLogName(_projectId, logEvent.SourceContext);

            var log = new LogEntry
            {
                LogName = _logName,
                Severity = TranslateSeverity(logEvent.Level),
                Timestamp = logEvent.LogTime.ToTimestamp(),
                TextPayload = logEvent.Message
            };

            var jsonPayload = new Struct();
            jsonPayload.Fields.Add("environment", Value.ForString(logEvent.Environment));

            var serviceContext = new Struct();
            serviceContext.Fields.Add("service", Value.ForString(logEvent.AppName));
            jsonPayload.Fields.Add("serviceContext", Value.ForStruct(serviceContext));

            if (logEvent.Exception != null)
            {
                jsonPayload.Fields.Add("exception_message", Value.ForString(logEvent.Exception.Message));

                if (string.IsNullOrWhiteSpace(logEvent.Exception.StackTrace) == false)
                    jsonPayload.Fields.Add("exception_stacktrace", Value.ForString(logEvent.Exception.StackTrace));
            }

            foreach (var item in logEvent.Properties)
            {
                jsonPayload.Fields.Add(item.Key, ConvertToWellKnowType(item.Value));
                CheckForSpecialProperties(log, item.Key, item.Value);
            }

            log.JsonPayload = jsonPayload;

            _client.WriteLogEntries(log.LogNameAsLogNameOneof, _resource, _loggingSinkOptions.Labels, new LogEntry[] { log });
        }

        public Value ConvertToWellKnowType(object propValue)
        {
            if (propValue == null)
                return Value.ForNull();

            if (propValue is bool)
                return Value.ForBool((bool)propValue);

            if (propValue is short || propValue is ushort || propValue is int || propValue is uint || propValue is long
                || propValue is ulong || propValue is float || propValue is double || propValue is decimal)
                return Value.ForNumber(Convert.ToDouble(propValue));

            return Value.ForString(propValue.ToString());
        }

        private void CheckForSpecialProperties(LogEntry log, string key, object value)
        {
            if (value != null && value is string)
            {
                if (_loggingSinkOptions.UseLogCorrelation && key.Equals(Constants.CORRELATIONID, StringComparison.OrdinalIgnoreCase))
                {
                    log.SpanId = value.ToString();
                    log.Trace = $"projects/{_projectId}/traces/{value.ToString()}";
                }

            }
        }

        public static string CreateLogName(string projectId, string name)
        {
            if (!LogNameCache.TryGetValue(name, out var logName))
            {
                // name must only contain: letters, numbers, underscore, hyphen, forward slash, period
                // limited to 512 characters and must be url-encoded
                var safeChars = LogNameUnsafeChars.Replace(name, "");
                var clean = Uri.EscapeDataString(safeChars);

                logName = new LogName(projectId, clean).ToString();

                LogNameCache.Add(name, logName);
            }
            return logName;
        }

        protected override void PushToStore(IEnumerable<LogEvent> logBatch)
        {
            foreach (var log in logBatch)
            {
                Send(log);
            }
        }
    }
}
