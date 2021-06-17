using System.Web.Mvc;

namespace TestWithDotnetFramework
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new TestLogging());
        }
    }
}
