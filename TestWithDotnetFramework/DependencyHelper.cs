using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;

namespace TestWithDotnetFramework
{
    public static class DependencyHelper
    {
        public static List<DelegatingHandler> GetMessageHandlers(this IDependencyResolver dependencyResolver)
        {
            return dependencyResolver.GetServices(typeof(DelegatingHandler)).Cast<DelegatingHandler>().ToList();
        }
        public static Stream GetLogFileStream()
        {
            var fullFilePath = string.Format(@"C:\Logs\log-{0}.txt", DateTimeOffset.UtcNow.ToString("yyyyMMdd"));
            if(File.Exists(fullFilePath))
                return File.OpenWrite(fullFilePath);
            return File.Create(fullFilePath);
        }
    }
}