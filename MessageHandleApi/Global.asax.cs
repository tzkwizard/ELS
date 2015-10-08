using System;
using System.Web.Http;

namespace MessageHandleApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        //private static ILogHandler _logHandler;
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(Autofac.Configure);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // _logHandler.CallMessageApi(HttpContext.Current.Request, HttpContext.Current.Response);         
        }
     
    }
}
