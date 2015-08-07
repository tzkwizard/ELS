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
using RenWeb.Framework.Interfaces.Utilities;


namespace WebApi
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static ILogHandler _logHandler;

        private static IComponentManager _globalComponentManager;
  
        public const string Userlog = "Userlog";

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

           // WebApiConfig.Register(GlobalConfiguration.Configuration);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            var bootStrap = new Bootstrap();
            _globalComponentManager = bootStrap.Initialize();

            _logHandler = _globalComponentManager.Container.GetExportedValue<ILogHandler>();
           
        }



      /*  void Application_AcquireRequestState(object sender, EventArgs e)
        {
            // Session is Available here
            HttpContext context = HttpContext.Current;
            context.Session["foo"] = "foo";
        }*/

        protected void Application_EndRequest(object sender, EventArgs e)
        {   
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
            esLog.ElsRequest =
                "[" + DateTime.Now.ToString("dd/MMM/yyyy:HH:mm:ss.FFF zz") + "]" + " \"" + request.HttpMethod + " "
                + request.Path + "\" " + response.StatusCode + " " + request.TotalBytes + " \"" +
                request.UrlReferrer + "\" " + "\"" + request.UserAgent + "\"" + " " + request.Form;




            Task.Run(() => ss(esLog));

      

            //_logHandler.CallMessageApi(request,response);

           // IRequestHandler ireRequestHandler=new RequestHandler();
            //IRequestHandler ireRequestHandler=new RequestHandler();
           // ireRequestHandler.CallMessageApi(request,response);
            
        }


        public async Task ss(ELSLogs esLog)
        {
            HttpClient httpClient = new HttpClient();
            //string dd = JsonConvert.SerializeObject(esLog);
            string uri = "https://microsoft-apiappdb7d3a2d2d8741749f28b72c31a1bb81.azurewebsites.net/api/LogMessage";
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