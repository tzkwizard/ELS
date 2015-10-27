using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using MessageHandleApi.App_Start;

namespace MessageHandleApi
{
    public static class WebApiConfig
    {
        private static void EnableCrossSiteRequests(HttpConfiguration config)
        {
            var cors = new EnableCorsAttribute(
                origins: "*",
                headers: "*",
                methods: "*");
            config.EnableCors(cors);
        }
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            //config.SetCorsPolicyProviderFactory(new CorsPolicyFactory());
            config.EnableCors();
            //EnableCrossSiteRequests(config);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            config.MessageHandlers.Add(new OptionFilter());
        }
    }
}
