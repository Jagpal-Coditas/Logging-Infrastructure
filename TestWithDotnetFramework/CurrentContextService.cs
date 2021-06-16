using Logging.Common;
using System.Web.Mvc;

namespace TestWithDotnetFramework
{
    public class CurrentContextService : ICurrentContextService
    {
        public ILoggerContext GetLoggerContext()
        {
            return DependencyResolver.Current.GetService<ILoggerContext>();
        }
    }
}