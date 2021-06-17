using System.Collections.Generic;

namespace Logging.Common
{
    public class SnakeCaseFormatter : BaseLogEventFormatter
    {
        public SnakeCaseFormatter()
        {

        }
        public SnakeCaseFormatter(ILogEventFormatter logEventFormatter) : base(logEventFormatter)
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
