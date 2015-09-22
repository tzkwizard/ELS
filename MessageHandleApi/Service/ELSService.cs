using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using MessageHandleApi.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using StackExchange.Redis;

namespace MessageHandleApi.Service
{
    public class ELSService : IELSService
    {

        private readonly CloudQueue _queue;
        private ConnectionMultiplexer _connection;
        private IDatabase _cache;
        private bool _queueError;


        public ELSService()
        {
            _queue = GetQueue();
        }
       

        public string GetMessage(ELSLogs esLog)
        {
            var userInfo = "null null";
            if (esLog.ElsUser != null)
            {
                userInfo = esLog.ElsUser.LastName + "." + esLog.ElsUser.FirstName + "----" + esLog.ElsUser.PersonID + " " + esLog.ElsUser.District + "----" + esLog.ElsUser.UserName;
               // + esLog.ElsUser.PersonID + "." + esLog.ElsUser.ConfigSchoolID 
                //    + "." + esLog.ElsUser.SchoolCode + "." + esLog.ElsUser.SchoolYearID
            }
            var requestMessage = "[" + esLog.ElsRequest.Time + "]" + " \"" + esLog.ElsRequest.HttpMethod + " "
                + esLog.ElsRequest.Path + "\" " + esLog.ElsRequest.StatusCode + " " + esLog.ElsRequest.TotalBytes + " \"" +
                esLog.ElsRequest.UrlReferrer + "\" " + "\"" + esLog.ElsRequest.UserAgent + "\"" + " " + esLog.ElsRequest.Form;

            var ipAddress = esLog.ElsRequest.IpAddress;
            var responseMessage = esLog.ElsResponse.StatusCode + esLog.ElsResponse.StatusDescription;

            return ipAddress + " " + userInfo + " " + requestMessage + "null " + responseMessage + " none";          
        }


        public async Task SendQueueAsyncAll(string message)
        {

            _queueError = false;

            _queueError = CheckQueue(_queueError);

            await SendQueueAsync(_queue, message, _queueError);

            //create multiple async task to send message to queue5
            /* for (var i = 0; i < num; i++)
             {
                 //Task.Run(() => SendQueueAsync(_queue, message, _queueError));
                
             }*/
        }

        private async Task SendQueueAsync(CloudQueue queue, string message, bool flag)
        {
            if (!flag)
            //create, fufill and send message
            {

                await queue.AddMessageAsync(new CloudQueueMessage(message));
            }
            else
            {

                await _cache.ListLeftPushAsync("s", message);
            }

        }

       private bool CheckQueue(bool queueError)
        {
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
                queueError = true;
            }
            return queueError;
        }

        private static CloudQueue GetQueue()
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
