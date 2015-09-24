using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using WebApi.Models;
using RenWeb.Framework.Interfaces;
using RenWeb.Framework;
using RenWeb.Framework.Utilities;
using Newtonsoft;
using System.Text;
using System.Net.Http.Headers;
using RenWeb.Framework.Client.Startup;
using RenWeb.Framework.Handlers;
using RenWeb.Framework.Interfaces.Client.Startup;
using RenWeb.Framework.Interfaces.Utilities;


namespace WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        //private static ILogHandler _logHandler;

        //private static IComponentManager _globalComponentManager;

        public const string Userlog = "Userlog";
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var bootStrap = new Bootstrap();
 /*           _globalComponentManager = bootStrap.Initialize();

           _logHandler = _globalComponentManager.Container.GetExportedValue<ILogHandler>();*/

            //_logHandler = new LogHandler();
            
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {

            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;


            ELSLogs esLog = new ELSLogs();
            esLog.ElsUser = new ElsUserInfromation();
            esLog.ElsResponse = response.StatusCode;
            esLog.ElsIpaddress = request.UserHostAddress;
            esLog.ElsUser.UserId = "Tz";
            esLog.ElsRequest =
                "[" + DateTime.Now.ToString("dd/MMM/yyyy:HH:mm:ss.FFF zz") + "]" + " \"" + request.HttpMethod + " "
                + request.Path + "\" " + response.StatusCode + " " + request.TotalBytes + " \"" +
                request.UrlReferrer + "\" " + "\"" + request.UserAgent + "\"" + " " + request.Form;

            if (string.IsNullOrEmpty(request.Form.ToString()))
            {
                esLog.ElsRequest += "null";}
            QueryInfo i = new QueryInfo {Type = "logs", Size = 10,SearchText = "Beijing",Start =DateTime.Now.AddDays(-33),End=DateTime.Now};
           

             //Task.Run(() => ss(esLog));
           // sp(i);


            // _logHandler.CallMessageApi(request,response);

            // IRequestHandler ireRequestHandler=new RequestHandler();
            //IRequestHandler ireRequestHandler=new RequestHandler();
            // ireRequestHandler.CallMessageApi(request,response);

        }
        private void sp(QueryInfo i)
        {
            HttpClient httpClient = new HttpClient();
            //string dd = JsonConvert.SerializeObject(esLog);
            string uri = "https://microsoft-apiapp463245e7d2084cb79dbc3d162e7b94cb.azurewebsites.net/query/StringQuery";
            var javaScriptSerializer = new
              System.Web.Script.Serialization.JavaScriptSerializer();
            string jsonString = javaScriptSerializer.Serialize(i);
            //Console.WriteLine(jsonString);
            // httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // await httpClient.PostAsJsonAsync(uri, jsonString);

             var x =httpClient.PostAsync(uri, new StringContent(jsonString, Encoding.UTF8, "application/json")).Result;
            
            var y = x;
        }


        private async Task ss(ELSLogs esLog)
        {
            HttpClient httpClient = new HttpClient();
            //string dd = JsonConvert.SerializeObject(esLog);
            string uri = "https://microsoft-apiapp463245e7d2084cb79dbc3d162e7b94cb.azurewebsites.net/api/LogMessage";
            var javaScriptSerializer = new
              System.Web.Script.Serialization.JavaScriptSerializer();
            string jsonString = javaScriptSerializer.Serialize(esLog);
            //Console.WriteLine(jsonString);
            // httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // await httpClient.PostAsJsonAsync(uri, jsonString);

            await httpClient.PostAsync(uri, new StringContent(jsonString, Encoding.UTF8, "application/json"));
        }



    }

}
