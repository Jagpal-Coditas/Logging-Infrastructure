using Logging.Abstraction.Services;
using System.Web.Mvc;

namespace TestWithDotnetFramework
{
    public class CurrentContextService : ICurrentContextService
    {
        public ILoggerContextService GetLoggerContext()
        {
            return DependencyResolver.Current.GetService<ILoggerContextService>();
        }
    }
}