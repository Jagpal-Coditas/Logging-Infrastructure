namespace Logging.Common
{
    public interface ICurrentContextService
    {
        ILoggerContext GetLoggerContext();
    }
}
