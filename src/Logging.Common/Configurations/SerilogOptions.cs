using System;

namespace Logging.Common.Configurations
{
    public class SerilogOptions
    {
        public string AppName { get; set; }
        public string Environment { get; set; }
        public const string Serilog = "serilog";
        public bool ConsoleEnabled { get; set; }
        public string Level { get; set; }
        public Func<ILoggerContext> GetLoggerContext { get; set; }
    }
}
