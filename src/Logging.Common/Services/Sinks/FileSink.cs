using Newtonsoft.Json;
using System;
using System.IO;
namespace Logging.Common
{
    public class FileSink : ISink
    {
        private readonly IFileSinkOptions _sinkOptions;
        public FileSink(IFileSinkOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(typeof(IFileSinkOptions).FullName);

            _sinkOptions = options;
        }
        public bool IsFailOverSink { get; set; }

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
    }
}
