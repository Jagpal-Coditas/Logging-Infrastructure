using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Logging.Common
{
    public class FlatenObjectFormatter : BaseLogEventFormatter
    {
        public FlatenObjectFormatter()
        {

        }
        public FlatenObjectFormatter(ILogEventFormatter logEventFormatter) : base(logEventFormatter)
        {
        }
        protected override LogEvent DoFormatting(LogEvent logEvent)
        {
            var propsWithFlatenObject = new Dictionary<string, object>();
            foreach (var prop in logEvent.Properties)
            {
                if (prop.Value.IsPrimitiveType())
                    propsWithFlatenObject.Add(prop.Key, prop.Value);
                else
                {
                    FlattenObjectAndAddProperties(propsWithFlatenObject, prop.Value);
                }
            }

            logEvent.Properties = propsWithFlatenObject;
            return logEvent;
        }

        void FlattenObjectAndAddProperties(IDictionary<string, object> propertiesList, object obj)
        {
            var jObject = JObject.Parse(JsonConvert.SerializeObject(obj));
            var flatObj = jObject.Flatten(false);
            foreach (var item in flatObj)
            {
                propertiesList.AddIfNotNullEmpty(item.Key, item.Value);
            }
        }
    }
}
