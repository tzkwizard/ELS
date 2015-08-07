﻿using System;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using WebApi.Models;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using System.Text.RegularExpressions;
using StackExchange.Redis;

namespace WebApi
{
    class TestLogHandler : ITestLogHandler
    {
        private const string Userlog = "Userlog";
        private CloudQueue _queue;
        private ConnectionMultiplexer _connection;
        private IDatabase _cache;
        private bool _queueError;

     

        public void Save(string ipAddress, HttpContext context, HttpRequest request, HttpResponse response)
        {
            ELSLog esLog = new ELSLog();
            esLog.ElsIpaddress = ipAddress;
            /*  var x = request.Path;

              x = Getapifunction(x);

              switch (request.HttpMethod)
              {
                  case "GET" :
                      x = "SEE " + x;
                      break;
                  case "POST":
                      x = "SENT " + x;
                      break;
              }*/
            // if (x == "/api/contact")
            // {
            //     x = "sendqueue";}
            esLog.ElsRequest = "[" + DateTime.Now.ToString("dd/MMM/yyyy:HH:mm:ss zz") + "]" + " \"" + request.HttpMethod + " "
                + request.Path + "\" " + response.StatusCode + " " + request.TotalBytes + " \"" + request.UrlReferrer + "\" " + "\"" + request.UserAgent + "\"" + " " + request.Form;

            /*    if (HttpContext.Current.Session != null)
                {
                   // context.Session[Userlog] = esLog;
                    Session[Userlog] = esLog;
                }*/
            context.Items[Userlog] = esLog;
        }

        public void Update(int responselog)
        {
            HttpContext context = HttpContext.Current;
            ELSLog esLog = new ELSLog();
            esLog = (ELSLog)context.Items[Userlog];
            esLog.ElsResponse = responselog;
        }

        public void SendQueueAsyncAll(int num)
        {
            _queueError = false;
            HttpContext context = HttpContext.Current;
            ELSLogs esLog = new ELSLogs();
            esLog = (ELSLogs)context.Items[Userlog];

            if (_queue == null)
            {
                _queue = GetQueue();
                if (_queue == null)
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
                    _queueError = true;
                }
            }

            //get Userlog object and fill with response information
           // String message = "";
          //  message += esLog.ElsIpaddress + " - - " + esLog.ElsRequest + " " + esLog.ElsResponse + " none";

            string message = "117.12.112.51 tzkwizards monster [13/Apr/2015:17:52:34.388 -05] \"POST /api/messagehandler/14\" 223 33 \"http://elswebapi.azurewebsites.net/\" \"Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2272.101 Safari/537.36\" spur 202 none";
            //create multiple async task to send message to queue5
            for (var i = 0; i < num; i++)
            {
                var i1 = i;
                Task.Run(() => SendQueueAsync(i1, esLog, _queue, message));
            }

        }
        public void Listqueue()
        {
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            Console.WriteLine("Create a list for queue ");


            foreach (var queue in queueClient.ListQueues())
            {
                Console.WriteLine(queue.Uri + "======" + queue.Name);
            }
        }


/// <summary>
/// 
/// </summary>
/// <param name="i"></param>
/// <param name="esLog"></param>
/// <param name="queue"></param>
/// <param name="message"></param>
/// <returns></returns>

        private async Task SendQueueAsync(int i, ELSLogs esLog, CloudQueue queue,string message)
        {
            if (!_queueError)
                //create, fufill and send message
            {
                
            await queue.AddMessageAsync(new CloudQueueMessage(message));
            }
            else { 

            _cache.ListLeftPush("s", message);
            }        

    }

        private CloudQueue GetQueue()
        {
            try
            {
                var storageAccount =
                    CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ToString());

                // Create a queue client for interacting with the queue service
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue = queueClient.GetQueueReference("elsqueue");
                queue.CreateIfNotExists();
                return queue;
            }
            catch (Exception)
            {

                return null;
            }
           
        }
       
        private string Getapifunction(string x)
        {
            Match cmatch = Regex.Match(x, @"api/contact/([A-Za-z0-9\-]+)$",
                RegexOptions.IgnoreCase);
            if (cmatch.Success)
            {
                x = "contact";
            }
            Match c2match = Regex.Match(x, @"api/contact$",
                RegexOptions.IgnoreCase);
            if (c2match.Success)
            {
                x = "contact";
            }
            Match dmatch = Regex.Match(x, @"api/users/diarires/([A-Za-z0-9\-]+)/entries/([A-Za-z0-9\-]+)$",
               RegexOptions.IgnoreCase);

            if (dmatch.Success)
            {
                x = "dairyentry";
            }

            Match d2match = Regex.Match(x, @"api/users/diarires/([A-Za-z0-9\-]+)$",
              RegexOptions.IgnoreCase);

            if (d2match.Success)
            {
                x = "dairydate";
            }








            return x;
        }

     
        private CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                Console.ReadLine();
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.");
                Console.ReadLine();
                throw;
            }

            return storageAccount;
        }

    }
}
