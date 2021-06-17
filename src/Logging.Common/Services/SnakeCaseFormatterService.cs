using Logging.Abstraction.Models;
using Logging.Abstraction.Services;
using System.Collections.Generic;

namespace Logging.Common.Services
{
    public class SnakeCaseFormatterService : BaseLogEventFormatterService
    {
        public SnakeCaseFormatterService()
        {

        }
        public SnakeCaseFormatterService(ILogEventFormatterService logEventFormatter) : base(logEventFormatter)
        {
        }
        protected override LogEvent DoFormatting(LogEvent logEvent)
        {
            var snakeCaseConvertedDic = new Dictionary<string, object>();
            foreach (var prop in logEvent.Properties)
            {
                snakeCaseConvertedDic.Add(prop.Key.ToSnakeCase(), prop.Value);
            }

            logEvent.Properties = snakeCaseConvertedDic;
            return logEvent;
        }
    }
}
