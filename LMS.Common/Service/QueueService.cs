using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using StackExchange.Redis;

namespace LMS.Common.Service
{
    public class QueueService : IQueueService
    {

        private readonly CloudQueue _queue;
        private ConnectionMultiplexer _connection;
        private IDatabase _cache;
        private bool _queueError;


        public QueueService()
        {
            var qName = "queue";
            _queue = GetQueue(qName);
        }


        public async Task SendToQueue(string m)
        {
           await SendQueueAsync(_queue, m, false);
        }


        private async Task SendQueueAsyncAll(string message)
        {

            _queueError = false;

            _queueError = CheckQueue(_queueError);

            await SendQueueAsync(_queue, message, _queueError);

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

        public CloudQueue GetQueue(string n)
        {
            try
            {
                var storageAccount =
                    CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ToString());

                // Create a queue client for interacting with the queue service
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue = queueClient.GetQueueReference(n);
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
