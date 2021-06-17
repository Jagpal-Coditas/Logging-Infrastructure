using Logging.Abstraction.Models;
using Logging.Abstraction.Services;
using Logging.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Web.Mvc;

namespace TestWithDotnetFramework
{
    public class TestLogging : ActionFilterAttribute
    {
        private const string RESPONSETIMEKEY = "ResponseTimeKey";
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Items[RESPONSETIMEKEY] = Stopwatch.StartNew();
            filterContext.HttpContext.Request.Headers.Add(Constants.HttpHeaders.CORRELATIONIDHEADER, Guid.NewGuid().ToString());
            StoreMvcInfoInHttpContext(filterContext);
            base.OnActionExecuting(filterContext);
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            Stopwatch stopwatch = (Stopwatch)filterContext.HttpContext.Items[RESPONSETIMEKEY];
            // Calculate the time elapsed   
            var timeElapsed = stopwatch.Elapsed.Milliseconds;
            var actionDescriptor = filterContext.ActionDescriptor;
            var actionName = actionDescriptor.ActionName;
            var controllerName = actionDescriptor.ControllerDescriptor.ControllerName;
            var statusCode = filterContext.HttpContext.Response.StatusCode;
            var _logger = DependencyResolver.Current.GetService<ILogger<TestLogging>>();
            _logger.LogInformation("REQUEST PROCESSED, CONTROLLER:{Controller}, ACTION: {Action}, RESPONSE TIME IS {ResponseTime}ms,StatusCode:{StatusCode},{IsLoggedFromMiddleware}", controllerName, actionName, timeElapsed, statusCode, true);
            _logger.LogInformation("Api log {@Apilog}", new ApiLog()
            {
                Api = controllerName,
                Verb = actionName,
                IsSuccessful = true,
                Url = filterContext.RequestContext.HttpContext.Request.RawUrl,
                Request = Encoding.UTF8.GetBytes("await GetRequestPayload(request)"),
                Response = Encoding.UTF8.GetBytes("await GetResponsePayload(response)")
            }, null, null);

            var tracedata = new Dictionary<string, string>();
            tracedata.Add("trace1", "trace");
            _logger.LogTrace("trace added {@TraceData}", tracedata);
            _logger.LogError(new EventId(), new ArgumentException("test exceptionlog"), "Exception occured in app");
            base.OnActionExecuted(filterContext);
        }
        private void StoreMvcInfoInHttpContext(ActionExecutingContext actionContext)
        {

            var actionDescriptor = actionContext.ActionDescriptor;
            var actionName = actionDescriptor.ActionName;
            var controllerName = actionDescriptor.ControllerDescriptor.ControllerName;
            string userName = null;
            if (actionContext.HttpContext.User.Identity.IsAuthenticated)
            {
                userName = actionContext.HttpContext.User.Identity.Name;
            }
            var requestType = string.Empty;
            var clientHostIP = string.Empty;
            var clientHostName = string.Empty;
            var browser = string.Empty;
            var browserVersion = string.Empty;
            var userSessionId = string.Empty;
            var origin = string.Empty;
            if (actionContext.HttpContext != null && actionContext.HttpContext.Request != null)
            {
                requestType = actionContext.HttpContext.Request.RequestType;
                clientHostIP = actionContext.HttpContext.Request.UserHostAddress;
                clientHostName = actionContext.HttpContext.Request.UserHostName;
                browser = actionContext.HttpContext.Request.Browser.Browser;
                browserVersion = actionContext.HttpContext.Request.Browser.Version;
                userSessionId = actionContext.HttpContext.Session.SessionID;
                origin = actionContext.HttpContext.Request.Url.ToString();
            }
            var _loggerContext = DependencyResolver.Current.GetService<ILoggerContextService>();
            _loggerContext.Set(LogContextProperty.Create(Constants.CORRELATIONID,
                  actionContext.HttpContext.Request.Headers.Get(Constants.HttpHeaders.CORRELATIONIDHEADER),
                  true));
            _loggerContext.Set(LogContextProperty.Create("requestType",
                  requestType));
        }
    }
}