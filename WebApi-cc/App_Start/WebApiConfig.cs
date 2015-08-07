using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Microsoft.Ajax.Utilities;
using WebApi.Services;

namespace WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //attribute routing
            config.MapHttpAttributeRoutes();


            config.Routes.MapHttpRoute(
                name: "Food",
                routeTemplate: "api/nutrition/foods/{foodid}",
                defaults: new {controller="Foods",id = RouteParameter.Optional }
                //constraints:new{id="/d+"}
            );
            config.Routes.MapHttpRoute(
                name: "Measures",
                routeTemplate: "api/nutrition/foods/{foodid}/measures/{id}",
                defaults: new { controller = "Measures", id = RouteParameter.Optional }
                //constraints:new{id="/d+"}
            );
//            config.Routes.MapHttpRoute(
//                name: "Measures2",
//                routeTemplate: "api/nutrition/foods/{foodid}/measures/{id}",
//                defaults: new { controller = "MeasuresV2", id = RouteParameter.Optional }
//                //constraints:new{id="/d+"}
//            );
            config.Routes.MapHttpRoute(
                name: "Diarires",
                routeTemplate: "api/users/diarires/{diaryid}",
                defaults: new { controller = "Diarires", diaryid = RouteParameter.Optional }
                //constraints:new{id="/d+"}
            );
            config.Routes.MapHttpRoute(
               name: "DiariresEntries",
               routeTemplate: "api/users/diarires/{diaryid}/entries/{id}",
               defaults: new { controller = "DiariresEntries", id = RouteParameter.Optional }
                //constraints:new{id="/d+"}
           );





            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
//            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
//            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
            

           config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            // To disable tracing in your application, please comment out or remove the following line of code
            // For more information, refer to: http://www.asp.net/web-api
            config.EnableSystemDiagnosticsTracing();



            //replace the contoller configuration 
            config.Services.Replace(typeof(IHttpControllerSelector),new WebApiControllerSelector(config));
        }
    }
}
