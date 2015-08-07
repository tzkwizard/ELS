using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Elasticsearch.Net.ConnectionPool;
using MessageHandleApi.Models;
using Nest;
using RenWeb.Framework.Handlers;
using RenWeb.Framework.Interfaces;

namespace MessageHandleApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static ILogHandler _logHandler;
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            _logHandler = new LogHandler();    
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // _logHandler.CallMessageApi(HttpContext.Current.Request, HttpContext.Current.Response);         
        }
     
    }
}
