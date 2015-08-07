/*using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using StackExchange.Redis;
using WebApi.Models;

namespace WebApi
{
    public class RequestHandler : IRequestHandler
    {        
        private ELSLogs esLog;
        private MessageHandleApi _client;
        private readonly SemaphoreSlim _syncLock = new SemaphoreSlim(1);
        //private readonly UserSession _session;
        public RequestHandler()
        {

            esLog = new ELSLogs();
        }

        /*  public RequestHandler(ICurrentSession curSession)
          {

              esLog = new ELSLog();
              _session = curSession.Session;
          }
        #1#

        public void CallMessageApi(HttpRequest request, HttpResponse response)
        {
            if (_client == null)
            {
                _client = new MessageHandleApi();
            }

            //eslog.userinformation 
            /*  var userData = new UserSessionInformation
              {
                 District = _session.DistrictCode,
                 ConfigSchoolID = _session.ConfigSchoolID,
                 AccountingIsDistrictWide = _session.AccountingIsDistrictWide,
                 DatabaseServer = _session.DatabaseServer,
                 LibraryIsDistrictWide = _session.LibraryIsDistrictWide,
                 PersonID = _session.PersonID,
                 SchoolCode = _session.SchoolCode,
                 SchoolYearID = _session.SchoolYearID
             };#1#
            String message = "";
            //esLog = new ELSLog();
            esLog.ElsResponse = response.StatusCode;
            esLog.ElsIpaddress = request.UserHostAddress;
            esLog.ElsRequest =
                "[" + DateTime.Now.ToString("dd/MMM/yyyy:HH:mm:ss.FFF zz") + "]" + " \"" + request.HttpMethod + " "
                + request.Path + "\" " + response.StatusCode + " " + request.TotalBytes + " \"" +
                request.UrlReferrer + "\" " + "\"" + request.UserAgent + "\"" + " " + request.Form;

            // message += esLog.ElsIpaddress + " - - " + esLog.ElsRequest + " " + esLog.ElsResponse + " none";


            Task.Run(() => Send());

        }

        private async Task Send()
        {

            try
            {
               
                // _client.LogMessage.Post(esLog);
                await _client.LogMessage.PostAsync(esLog);

            }
            catch (Exception)
            {
                const string log = @"C:\TempELSlog.txt";
                if (File.Exists(log))
                {

                    using (StreamWriter file = File.AppendText(@"C:\TempELSlog.txt"))
                    {

                        file.WriteLine(DateTime.Now.ToLongTimeString() + "-----" + esLog.ElsIpaddress + esLog.ElsRequest + esLog.ElsResponse);
                    }
                }
                else
                {
                    using (StreamWriter file = new StreamWriter(@"C:\TempELSlog.txt"))
                    {
                        file.WriteLine(DateTime.Now.ToLongTimeString()+"-----"+esLog.ElsIpaddress + esLog.ElsRequest + esLog.ElsResponse);
                    }
                }
            }
        }



     


    }
}*/