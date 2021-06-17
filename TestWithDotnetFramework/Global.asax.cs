﻿using Autofac;
using Autofac.Integration.Mvc;
using Logging.Common;
using Logging.Net45;
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
            builder.RegisterType<LoggerContext>().As<ILoggerContext>().InstancePerLifetimeScope();

            builder.Register(c =>
            {
                return new SnakeCaseFormatter(new FlatenObjectFormatter());
            }).As<ILogEventFormatter>().SingleInstance();


            builder.Register(c =>
            {
                return new GoogleCloudLoggingSinkOptions("possible-point-316909", "global", null, null, null, true, true, "");
            }).As<IGoogleCloudLoggingSinkOptions>().SingleInstance();


            builder.RegisterType<GoogleCloudLoggingSink>().As<ISink>().InstancePerLifetimeScope();

            builder.Register(c =>
            {
                var sinks = new System.Collections.Generic.List<ISink>();
                sinks.Add(c.Resolve<ISink>());
                return ApplicationLoggerOptions.Create("ConnectAndSell", "QA", LogLevel.Information, sinks);
            }).As<IApplicationLoggerOptions>().SingleInstance();


            builder.Register(sc =>
            {
                var lf = new LoggerFactory();
                lf.AddProvider(new ApplicationLoggerProvider(sc.Resolve<IApplicationLoggerOptions>(), sc.Resolve<ICurrentContextService>()));
                return lf;
            }).As<ILoggerFactory>().InstancePerLifetimeScope();


            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();
            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}