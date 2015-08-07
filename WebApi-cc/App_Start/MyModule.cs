using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using WebApi.Models;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Providers.Entities;
using System.Web.Routing;
using System.Web.UI.WebControls;
using StackExchange.Redis;
using WebApi.App_Start;

namespace WebApi
{
    public class MyModule : IHttpModule
    {
       // private ILogHandler _ilogHandler;
        private const string Userlog = "Userlog";
        private ConnectionMultiplexer _connection;
        private IDatabase _cache;



        public void Init(HttpApplication objApplication)
        {
         // Register event handler of the pipe line
            objApplication.BeginRequest += new EventHandler(this.context_BeginRequest);
            objApplication.EndRequest += new EventHandler(this.context_EndRequest);
            
            
        }

        public void Dispose()
        {
        }

        private void context_EndRequest(object sender, EventArgs e)
        {
           /* HttpApplication application = (HttpApplication) sender;
            HttpContext context = application.Context;
            HttpResponse response = context.Response;
            HttpRequest request = context.Request;
            ELSLogs esLog = new ELSLogs();
           

            if (context.Items[Userlog] != null)
            {
            esLog = (ELSLogs) context.Items[Userlog];
            esLog.ElsResponse = response.StatusCode;
            String message = "";
            message += esLog.ElsIpaddress + " - - " + esLog.ElsRequest + " " + esLog.ElsResponse + " none";

                try
                {
                    string s = ConfigurationManager.ConnectionStrings["Azureapi"].ToString();
                   
                    var client = new MessageHandleApi(s);
                    client.LogMessage.Post(esLog);
                   // ITestLogHandler ilogHandler = new TestLogHandler();
                    //ilogHandler.SendQueueAsyncAll(1);

                }
                catch (Exception)
                {                   

                   Task.Run(() => Redis(message));

                }

           }*/
    }

        private async Task Redis(string message)
        {
            if (_connection == null)
            {
                _connection =
                    ConnectionMultiplexer.Connect(ConfigurationManager.ConnectionStrings["AzureRedis"].ToString());
                if (_cache == null)
                {
                    _cache = _connection.GetDatabase();

                }
            }
           await _cache.ListLeftPushAsync("s", message);
        }

        private void context_BeginRequest(object sender, EventArgs e)
        {
           /* HttpApplication application = (HttpApplication) sender;
            HttpContext context = application.Context;
            HttpResponse response = context.Response;
            HttpRequest request = context.Request;
            string ipAddress = context.Request.UserHostAddress;
            if (request.HttpMethod != "GET")
            {
                ELSLogs esLog = new ELSLogs
                {
                    ElsIpaddress = ipAddress,
                    ElsRequest =
                        "[" + DateTime.Now.ToString("dd/MMM/yyyy:HH:mm:ss.FFF zz") + "]" + " \"" + request.HttpMethod + " "
                        + request.Path + "\" " + response.StatusCode + " " + request.TotalBytes + " \"" +
                        request.UrlReferrer + "\" " + "\"" + request.UserAgent + "\"" + " " + request.Form
                };
                context.Items[Userlog] = esLog;
            }

*/

            /* if (request.HttpMethod != "GET")
            {
                ILogHandler ilogHandler = new LogHandler();
             //ILogHandler ilogHandler = new LogHandler();
                    // ilogHandler.Update(responselog);
                    // ilogHandler.SendQueueAsyncAll(1);
            ilogHandler.Save(ipAddress, context, request, response);
        }*/

        }






    }
}