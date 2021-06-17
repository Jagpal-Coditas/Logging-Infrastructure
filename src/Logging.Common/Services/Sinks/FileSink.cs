using Logging.Common.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
namespace Logging.Common
{
    public class FileSink : BaseSink, ISink
    {
        private readonly IFileSinkOptions _sinkOptions;
        public FileSink(IFileSinkOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(typeof(IFileSinkOptions).FullName);

            _sinkOptions = options;
        }
        public override bool IsFailOverSink { get { return true; } }

        public override ILogEventFormatter LogFormatter
        {
            get
            {
                return new SnakeCaseFormatter();
            }
        }

        public override bool IsPrioritySink => throw new NotImplementedException();

        public void Push(LogEvent logEvent)
        {
            var logString = JsonConvert.SerializeObject(logEvent);

            using (var fileStream = _sinkOptions.GetFileStream())
            {
                using (var streamWriter = new StreamWriter(fileStream, System.Text.Encoding.UTF8))
                {
                    streamWriter.WriteLine(logString);
                }
            }
        }

        protected override void HandleLogEvent(LogEvent logEvent)
        {
            throw new NotImplementedException();
        }

        protected override void PushToStore(IEnumerable<LogEvent> logBatch)
        {
            throw new NotImplementedException();
        }
    }
}
