using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace Logging.Common
{
    public static class LoggingHelper
    {
        public static string GetHeaderIfPresent(HttpRequestHeaders headers, string headerName)
        {
            if (headers.Contains(headerName))
            {
                return headers.GetValues(headerName).First();
            }
            return string.Empty;
        }
        public static void AddIfNotNullEmpty(this IDictionary<string, object> props, string key, object value)
        {
            if (props == null || value == null)
                return;

            if (value is string && string.IsNullOrWhiteSpace(value.ToString()))
                return;

            props.Add(key.ToSnakeCase(), value);
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

    }

}
