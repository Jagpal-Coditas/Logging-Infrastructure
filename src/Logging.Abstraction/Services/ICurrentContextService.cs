namespace Logging.Abstraction.Services
{
    public interface ICurrentContextService
    {
        ILoggerContextService GetLoggerContext();
    }
}
