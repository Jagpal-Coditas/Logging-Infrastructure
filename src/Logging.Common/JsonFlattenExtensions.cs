using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Logging.Common
{
    public static class JsonFlattenExtensions
    {
        public static IDictionary<string, object> Flatten(this JObject jsonObject, bool includeNullAndEmptyValues = true) => jsonObject
                .Descendants()
                .Where(p => !p.Any())
                .Aggregate(new Dictionary<string, object>(), (properties, jToken) =>
                {
                    var value = (jToken as JValue)?.Value;

                    if (!includeNullAndEmptyValues)
                    {
                        if (value?.Equals("") == false)
                        {
                            properties.Add(jToken.Path, value);
                        }
                        return properties;
                    }

                    var strVal = jToken.Value<object>()?.ToString().Trim();
                    if (strVal?.Equals("[]") == true)
                    {
                        value = Enumerable.Empty<object>();
                    }
                    else if (strVal?.Equals("{}") == true)
                    {
                        value = new object();
                    }

                    properties.Add(jToken.Path, value);

                    return properties;
                });


        public static T Get<T>(this IDictionary<string, object> dictionary, string key)
        {
            return (T)dictionary[key];
        }

        public static void Set(this IDictionary<string, object> dictionary, string key, object value)
        {
            dictionary[key] = value;
        }

        public static bool TryGet<T>(this IDictionary<string, object> dictionary, string key, out T value)
        {
            object result;
            if (dictionary.TryGetValue(key, out result) && result is T)
            {
                value = (T)result;
                return true;
            }
            value = default(T);
            return false;
        }
    }
}