using System;
using System.Configuration;
using System.Globalization;
using System.Threading.Tasks;
using Elasticsearch.Net.ConnectionPool;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Nest;
using StackExchange.Redis;

namespace WebJob.QueueAndRedis
{
    class Queuestore
    {
        public static Uri Node;
        public static ConnectionSettings Settings;
        public static ElasticClient Client;




        public void Redis(string message)
        {

            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect("avmredis.redis.cache.windows.net,password=L+XyW+nDc0GbQcHaC8g8jpW8ACUuwYcF4wTQmQnsbRo=");
            IDatabase cache = connection.GetDatabase();
            cache.ListLeftPush("s", message);
          
        }


        public void CreateIndex(string name)
        {



            var indexsettings = new IndexSettings();
            indexsettings.NumberOfReplicas = 1;
            indexsettings.NumberOfShards = 1;
            Client.CreateIndex(c => c
                .Index(name)
                .InitializeUsing(indexsettings)
                .AddMapping<ActivityLog>(m => m.MapFromAttributes()));
        }

        public  void InsertData(string message)
        {
           // Node = new Uri("http://avm1:9200/");
            Node = new Uri("http://localhost:9200/");
            var connectionPool = new SniffingConnectionPool(new[] { Node });  
            Settings = new ConnectionSettings(Node, defaultIndex: "azure");
            Client = new ElasticClient(Settings);
            string date = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            var newPost = new ActivityLog(message, date, "");

            Redis(message);

           //Client.Index(newPost);

            //Console.WriteLine("message delete and forward");
           //queue.DeleteMessage(message);
        }
        /// <summary>
        /// Create a queue for the sample application to process messages in. 
        /// </summary>
        /// <returns>A CloudQueue object</returns>

        //get and delete message
//        public async Task ProcessMessageAsyncAll(string queuename)
//        {
//            await Task.Run(() => ProcessMessageAsync(queuename));
//            
//        }

        public async Task ProcessAsync(string queuename)
        {
             await Task.Run(() => ProcessMessage(queuename));

             
        }


        public void ProcessMessage(string queuename)
        {

            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ToString());
            // Create a queue client for interacting with the queue service
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            CloudQueue queue = queueClient.GetQueueReference("qqwwss");
           // Console.WriteLine("1");
           //CloudQueueMessage message = queue.PeekMessage();
            int j = 1;
            j++;
            Console.WriteLine(j);
            CloudQueueMessage message = queue.GetMessage();
            queue.DeleteMessage(message);
         
            InsertData(message.AsString);
        }

        public void ProcessMessageAsync(CloudQueue queue)
        {
            
            int i = 1;
            int j = 1;
            j++;
            foreach (CloudQueueMessage message in queue.GetMessages(20 , TimeSpan.FromSeconds(30), null, null))
            {
                // Process all messages in less than 5 minutes, deleting each message after processing.
                
                Console.WriteLine("message delete and forward --" + "///" + i);
                
                //var message1 = message;
                string m = message.AsString;
                Console.WriteLine("message  --"+m + "///" + i);
               // await Task.Run(() => InsertData("",queue));
                InsertData(m);
               queue.DeleteMessage(message);
                i++;
            }
           
            
        }


        /// <summary>
        /// Demonstrate basic queue operations such as adding a message to a queue, peeking at the front of the queue and dequeing a message.
        /// </summary>
        /// <param name="queue">The sample queue</param>
        public async Task BasicQueueOperationsAsync(CloudQueue queue)
        {
            // Insert a message into the queue using the AddMessage method. 
            Console.WriteLine("2. Insert a single message into a queue");
            for(int i=0;i<10;i++)
            {await queue.AddMessageAsync(new CloudQueueMessage("Hello World!"));}

            // Peek at the message in the front of a queue without removing it from the queue using PeekMessage (PeekMessages lets you peek >1 message)
            Console.WriteLine("3. Peek at the next message");
            CloudQueueMessage peekedMessage = await queue.PeekMessageAsync();
            if (peekedMessage != null)
            {
                Console.WriteLine("The peeked message is: {0}", peekedMessage.AsString);
            }

            // You de-queue a message in two steps. Call GetMessage at which point the message becomes invisible to any other code reading messages 
            // from this queue for a default period of 30 seconds. To finish removing the message from the queue, you call DeleteMessage. 
            // This two-step process ensures that if your code fails to process a message due to hardware or software failure, another instance 
            // of your code can get the same message and try again. 
            /* Console.WriteLine("4. De-queue the next message");
             CloudQueueMessage message = await queue.GetMessageAsync();
             if (message != null)
             {
                 Console.WriteLine("Processing & deleting message with content: {0}", message.AsString);
                 await queue.DeleteMessageAsync(message);
             }*/
        }

        /// <summary>
        /// Update an enqueued message and its visibility time. For workflow scenarios this could enable you to update 
        /// the status of a task as well as extend the visibility timeout in order to provide more time for a client 
        /// continue working on the message before another client can see the message. 
        /// </summary>
        /// <param name="queue">The sample queue</param>
        public async Task UpdateEnqueuedMessageAsync(CloudQueue queue)
        {
            // Insert another test message into the queue 
            Console.WriteLine("5. Insert another test message ");
            await queue.AddMessageAsync(new CloudQueueMessage("Hello World Again!"));

            Console.WriteLine("6. Change the contents of a queued message");
            CloudQueueMessage message = await queue.GetMessageAsync();
            message.SetMessageContent("Updated contents.");
            await queue.UpdateMessageAsync(
                message,
                TimeSpan.Zero,  // For the purpose of the sample make the update visible immediately
                MessageUpdateFields.Content |
                MessageUpdateFields.Visibility);
        }

        /// <summary>
        /// Demonstrate adding a number of messages, checking the message count and batch retrieval of messages. During retrieval we 
        /// also set the visibility timeout to 5 minutes. Visibility timeout is the amount of time message is not visible to other 
        /// clients after a GetMessageOperation assuming DeleteMessage is not called. 
        /// </summary>
        /// <param name="queue">The sample queue</param>
        public  async Task ProcessBatchOfMessagesAsync(CloudQueue queue)
        {
            // Enqueue 20 messages by which to demonstrate batch retrieval
            Console.WriteLine("7. Enqueue 20 messages.");
            for (int i = 0; i < 20; i++)
            {
                await queue.AddMessageAsync(new CloudQueueMessage(string.Format("{0} - {1}", i, "Hello World")));
            }

            // The FetchAttributes method asks the Queue service to retrieve the queue attributes, including an approximation of message count 
            Console.WriteLine("8. Get the queue length");
            queue.FetchAttributes();
            int? cachedMessageCount = queue.ApproximateMessageCount;
            Console.WriteLine("Number of messages in queue: {0}", cachedMessageCount);

            // Dequeue a batch of 21 messages (up to 32) and set visibility timeout to 5 minutes. Note we are dequeuing 21 messages because the earlier
            // UpdateEnqueuedMessage method left a message on the queue hence we are retrieving that as well. 
            Console.WriteLine("9. Dequeue 21 messages, allowing 5 minutes for the clients to process.");
            foreach (CloudQueueMessage msg in await queue.GetMessagesAsync(21, TimeSpan.FromMinutes(5), null, null))
            {
                Console.WriteLine("Processing & deleting message with content: {0}", msg.AsString);

                // Process all messages in less than 5 minutes, deleting each message after processing.
                await queue.DeleteMessageAsync(msg);
            }
        }
    }
}
