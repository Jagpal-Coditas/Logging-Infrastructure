using Autofac;
using Autofac.Integration.Mvc;
using Logging.Abstraction.Configuration;
using Logging.Abstraction.Services;
using Logging.Common;
using Logging.Common.Configurations;
using Logging.Common.Services;
using Microsoft.Extensions.Logging;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace TestWithDotnetFramework
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            var builder = new ContainerBuilder();


            builder.RegisterType<CurrentContextService>().As<ICurrentContextService>().InstancePerLifetimeScope();
            builder.RegisterType<ILoggerContextService>().As<ILoggerContextService>().InstancePerLifetimeScope();

            builder.Register(c =>
            {
                return new SnakeCaseFormatterService(new FlatenObjectFormatterService());
            }).As<ILogEventFormatterService>().SingleInstance();


            builder.Register(c =>
            {
                return new GoogleCloudLoggingSinkOptions("", "global", null, null, null, true, true, "");
            }).As<IGoogleCloudLoggingSinkOptions>().SingleInstance();


            builder.RegisterType<GoogleCloudLoggingSink>().As<ISinkService>().SingleInstance();

            builder.Register(c =>
            {
                var sinks = new System.Collections.Generic.List<ISinkService>();
                sinks.Add(c.Resolve<ISinkService>());
                return ApplicationLoggerOptions.Create("ConnectAndSell", "QA", LogLevel.Information, sinks);
            }).As<IApplicationLoggerOptions>().SingleInstance();


            builder.Register(sc =>
            {
                var lf = new LoggerFactory();
                lf.AddProvider(new ApplicationLoggerProviderService(sc.Resolve<IApplicationLoggerOptions>(), sc.Resolve<ICurrentContextService>()));
                return lf;
            }).As<ILoggerFactory>().InstancePerLifetimeScope();


            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();
            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}