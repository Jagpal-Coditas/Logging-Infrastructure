using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Coditas.Common.Logging
{
    public static class Utility
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

        public static string ToSnakeCase(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            var builder = new StringBuilder(text.Length + Math.Min(2, text.Length / 5));
            var previousCategory = default(UnicodeCategory?);

            for (var currentIndex = 0; currentIndex < text.Length; currentIndex++)
            {
                var currentChar = text[currentIndex];
                if (currentChar == '_')
                {
                    builder.Append('_');
                    previousCategory = null;
                    continue;
                }

                var currentCategory = CharUnicodeInfo.GetUnicodeCategory(currentChar);
                switch (currentCategory)
                {
                    case UnicodeCategory.UppercaseLetter:
                    case UnicodeCategory.TitlecaseLetter:
                        if (previousCategory == UnicodeCategory.SpaceSeparator ||
                            previousCategory == UnicodeCategory.LowercaseLetter ||
                            previousCategory != UnicodeCategory.DecimalDigitNumber &&
                            previousCategory != null &&
                            currentIndex > 0 &&
                            currentIndex + 1 < text.Length &&
                            char.IsLower(text[currentIndex + 1]))
                        {
                            builder.Append('_');
                        }

                        currentChar = char.ToLower(currentChar);
                        break;

                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.DecimalDigitNumber:
                        if (previousCategory == UnicodeCategory.SpaceSeparator)
                        {
                            builder.Append('_');
                        }
                        break;

                    default:
                        if (previousCategory != null)
                        {
                            previousCategory = UnicodeCategory.SpaceSeparator;
                        }
                        continue;
                }

                builder.Append(currentChar);
                previousCategory = currentCategory;
            }

            return builder.ToString();
        }

        internal static Dictionary<string, object> GetFormattedKeyValues(string name, object value)
        {
            var formattedValues = new Dictionary<string, object>();
            if (IsPrimitiveType(value))
            {
                formattedValues.Add(name.ToSnakeCase(), value);
            }
            else
            {
                var jObject = JObject.Parse(JsonConvert.SerializeObject(value));
                var flatObj = jObject.Flatten(false);
                foreach (var item in flatObj)
                {
                    formattedValues.Add(item.Key.ToSnakeCase(), item.Value);
                }
            }
            return formattedValues;
        }
        private static bool IsPrimitiveType(object value)
        {
            if (value == null)
                return false;

            var type = value.GetType();
            return type.Namespace == "System" || type.Namespace.StartsWith("System");
        }
    }
}
