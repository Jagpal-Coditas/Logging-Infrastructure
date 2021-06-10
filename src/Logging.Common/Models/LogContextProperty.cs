namespace Logging.Common.Models
{
    public class LogContextProperty
    {
        public LogContextProperty()
        {

        }
        private LogContextProperty(string key, string value)
        {
            Key = key;
            Value = value;
        }
        private LogContextProperty(string key, string value, bool isCommonLog) : this(key, value)
        {
            IsCommonLog = isCommonLog;
        }
        public string Key { get; set; }
        public string Value { get; set; }
        public bool IsCommonLog { get; set; }
        public static LogContextProperty Create(string key, string value)
        {
            return new LogContextProperty(key, value);
        }
        public static LogContextProperty Create(string key, string value, bool isCommonLog)
        {
            return new LogContextProperty(key, value, isCommonLog);
        }
    }
}
