namespace Logging.Common.Configurations
{
    public class SerilogOptions
    {
        public const string Serilog = "serilog";
        public bool ConsoleEnabled { get; set; }
        public string Level { get; set; }
    }
}
