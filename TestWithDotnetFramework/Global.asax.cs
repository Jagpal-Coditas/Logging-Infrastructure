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
                return new GoogleCloudLoggingSinkOptions("possible-point-316909", "global", null, null, null, true, true, @"{
                   'type':'service_account',
                   'project_id':'possible-point-316909',
                   'private_key_id':'60579c90a051d696868f3dc06fc507344e79e1a7',
                   'private_key':'-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQDOD7vSBJXHaqmI\ncBRhGiQQ31GUvZ76ztle8N8MjmhvVZvrYAy+KR5PcjrKRf+HyvowmtkMEVRjk2UF\nTjDklPaGbU8Uv8A6c9oYY5jGi/gjOyvOvZ38MN1iE+nG8mTs21JeD6Q2GGQ7twlU\n1TvNt04GyzD9pCpK7wBwzQjgwf03w6YVIQjWDRZHWJsr1VQTe5RYiO9khjqhy0do\nxe+Ssa7uAtY+QxH8CzSPf/c8Xo+gut89lxOYU92pRRJ44+ysgesIcglEZsKEGFqy\ncM15Ep9Ls7xCtF4MHlrxfLma8/HchaWfBBntCiJ7ISlTPUtyRTvjoKqq6l8iJOtB\ndqa5l2vZAgMBAAECggEAG7cxtS5TslZOMOFDrx8pJwclKG3sHQzxtMY4zK4M2Ei5\n/N8sPO7ij59WzqUP7m+mgMobE8ydk/pxFdUYrhi7zdnppJSj94NknBotTtXoYrNk\nkYf0i2/2dD4nF/tmCBZPI61T53NsFpITFGYihz62pSLzAcfxE6dw4OXbDKwFnIMb\nZdnGVeY9RNAadWbVtKHKkshEGX3AzNQcu1Mp9inobjWmFrJp20b4q5S+KSwTfKQB\nDAMLVFYrmqpVULwfBOpMpI3z/W08Q0Wf0ucko6YDBzMn4x2QXfgcCaik306a9fET\nO4L9ulzlF+KqYD3FM5wA6QOJP7BTq6ucgDxd1sQ1NwKBgQD6kbEiWjQUm0wKFk/q\nyFa5hQzIkudey6zy9hHQcD2bKEqjpsQn2KBV7tjXZqAjGpXfNygLAG/cXglIKZM1\nbOaACf0c9KNcateBBj+DuwU4TAwPy9s3BjaCcCUZ9NdZekV5KKexdw+fetO3ZD9k\ng2WaKMSYk854D2STejhQ0NEi8wKBgQDShxYnwCIA4a8BSZqtNEr53+os+Ry5E12c\nTLhBsdZctm+yBc6FmqT/ps11IjWS1+mUY+1Dol14TE4aDptYpKsvoHRu1GG0s1L9\nMerMEwAifMRofkyG5jd6g0mjlx8yR9TFfxTioUz9/ZWcj2fYDXabFWn9tzSWaMeq\niaxudJ6xAwKBgQCs5fdagu/JAFf55eeDVHYzUZG+nU+148kVZaJpN+nqtGS8hAh6\njkokwcky6Qe68U9VVVP2M3j8kI7LEpUXmmt7Emlrn4tR7A+EYnFVTqmNbTwtcjdz\nwn++he+z93TZsztZHSqJlGRdYX3R4AS2MMzdESCHQAaKZeS6tu1BJrYI7QKBgFCC\n1sc25AogRNQiOOP6Np4esqimUAS7UjZe5KxC/W6RWl/jdpqDjIw0Vyhvf7t7lNlp\n6afO5R4HWGsAoQoiV1EsdLqZwA/h8F/iAiAvOAL4YghwHIYObrMMmFHWjlilPcqV\nkgAlnZMYsmmgMh9e7rfaVaFwucy3n3wpkGbhdHrPAoGABrIChcU+bFli85N8kjVO\nM00/M+MPFAOrz+ib4qeNRLPgWi9SCs17W0vN4DhQeurMYTHogStdx+JBbIle02af\n0cEMefJrdVPfUsnUC1Iuah/e+r8nRJoqDSoWQen829T/AbkZICyRXFdXKnWFvUOD\nqs2RPNUlGAY5ecQ0H6kEgRk=\n-----END PRIVATE KEY-----\n',
                   'client_email':'log-testing@possible-point-316909.iam.gserviceaccount.com',
                   'client_id':'108283092557380460232',
                   'auth_uri':'https://accounts.google.com/o/oauth2/auth',
                   'token_uri':'https://oauth2.googleapis.com/token',
                   'auth_provider_x509_cert_url':'https://www.googleapis.com/oauth2/v1/certs',
                   'client_x509_cert_url':'https://www.googleapis.com/robot/v1/metadata/x509/log-testing%40possible-point-316909.iam.gserviceaccount.com'
                  }");
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