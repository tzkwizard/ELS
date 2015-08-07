using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace WebApi.Services
{
    public class WebApiControllerSelector:DefaultHttpControllerSelector
    {
        private HttpConfiguration _config;

        public WebApiControllerSelector(HttpConfiguration config):base(config)
        {
            _config = config;
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var controllers = GetControllerMapping();
            var routeData = request.GetRouteData();
            var controllerName = (string) routeData.Values["controller"];
            HttpControllerDescriptor descriptor;
            if (string.IsNullOrWhiteSpace(controllerName))
            {
                return base.SelectController(request);
            }


            else if (controllers.TryGetValue(controllerName, out descriptor))
            {
                //var version = "2";
                var version = GetversionFromQueryString(request);
                //var version = GetversionFromHeader(request);
                var newName = string.Concat(controllerName, "V", version);
                HttpControllerDescriptor versionedDescriptor;
                if (controllers.TryGetValue(newName, out versionedDescriptor))
                {
                    return versionedDescriptor;
                }
                return descriptor;

            }
            return null;
        }

        private string GetversionFromHeader(HttpRequestMessage request)
        {
            throw new NotImplementedException();
        }

        private string GetversionFromQueryString(HttpRequestMessage request)
        {
            var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var version = query["v"];
            if (version != null)
            {
                return version;
            }
            return "1";
        }
    }
}