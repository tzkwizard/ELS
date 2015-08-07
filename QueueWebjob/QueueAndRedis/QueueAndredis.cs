using System;
using System.Configuration;
using System.Globalization;
using Elasticsearch.Net.ConnectionPool;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Nest;
using StackExchange.Redis;


namespace QueueWebjob.QueueAndRedis
{
    public class QueueAndredis:IQueueAndredis
    {
        public static Uri Node;
      
        private ConnectionMultiplexer _connection;
        private IDatabase _cache;
        public void InsertData(string message)
        {
           /*
            //Send the raw message to elasticsearch
            Node = new Uri("http://localhost:9200/");
            var connectionPool = new SniffingConnectionPool(new[] { Node });
            Settings = new ConnectionSettings(Node, defaultIndex: "azure");
            Client = new ElasticClient(Settings);
            string date = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            var newPost = new ActivityLog(message, date, "");*/

            Redis(message);
        }


        public void ProcessMessageAsync(CloudQueue queue)
        {
            int i = 1;
            foreach (CloudQueueMessage message in queue.GetMessages(20, TimeSpan.FromSeconds(30), null, null))
            {
                // Process all messages in less than 30s, deleting each message after processing.
                string m = message.AsString;
                Console.WriteLine("message  --" + m + "///" + i);              
                InsertData(m);
                queue.DeleteMessage(message);
                i++;
            }
        }

        private void Redis(string message)
        {
            if (_connection == null)
            {
                _connection = ConnectionMultiplexer.Connect(ConfigurationManager.ConnectionStrings["AzureRedis"].ToString());
                if (_cache == null)
                {
                    _cache = _connection.GetDatabase();
                }
            }
                _cache.ListLeftPush("s", message);
            
        }

        public CloudQueue GetQueue()
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ToString());
            // Create a queue client for interacting with the queue service
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("elsqueue");
            return queue;
        }
    }
}
