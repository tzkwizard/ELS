using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Ninject.Activation;
using WebApi.Models;

namespace WebApi
{
    class Queuestore
    {
        public const string Userlog = "Userlog";

        //Send message
        public void SendQueueAsyncAll(int num, int responselog)
        {
            HttpContext context = HttpContext.Current;
            ELSLog esLog = new ELSLog();
            esLog = (ELSLog)context.Items[Userlog];
            esLog.ElsResponse = responselog;

            /* if (context.Session != null)
            {
                esLog = (ELSLog)context.Session[Userlog];
            }
            string ipAddress = context.Request.UserHostAddress;
           IPAddress[] addresslist = Dns.GetHostAddresses(request.Headers.Host);
             string ad="";
             foreach (IPAddress ip in addresslist)
             {
                 if (ip.AddressFamily == AddressFamily.InterNetwork)
                 {
                     ad  += ip.ToString();
                 }
             }*/
            
            for (var i = 0; i < num; i++)
            {
                //task = SendQueueAsync(i,request);

                var i1 = i;
                Task.Run(() => SendQueueAsync(i1, esLog));
            }

        }

        public async Task SendQueueAsync(int i, ELSLog esLog)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ToString());
            // Create a queue client for interacting with the queue service
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            String message = "";
            message += esLog.ElsIpaddress + " - - " + esLog.ElsRequest;
            //+ request.UserHostAddress + "~~~" + request.RawUrl + "~~~" + request.RequestContext + "~~~" + request.ServerVariables + "~~~" + request.GetBufferedInputStream() + "~~~" + request.RequestType + "~~~" + r;

            CloudQueue queue = queueClient.GetQueueReference("qqwwss");
            await queue.AddMessageAsync(new CloudQueueMessage(message));
            Console.WriteLine("message send" + i);
        }













        /// <summary>
        /// Create a queue for the sample application to process messages in. 
        /// </summary>
        /// <returns>A CloudQueue object</returns>
        public async Task<CloudQueue> CreateQueueAsync(string queuename)
        {
            // Retrieve storage account information from connection string.
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(CloudConfigurationManager.GetSetting("StorageConnectionString"));    
            // Create a queue client for interacting with the queue service
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            Console.WriteLine("Create a queue for the demo  "+queuename);
            CloudQueue queue = queueClient.GetQueueReference(queuename);
            try
            {
                await queue.CreateIfNotExistsAsync();
            }
            catch (StorageException)
            {
                Console.WriteLine("If you are running with the default configuration please make sure you have started the storage emulator. Press the Windows key and type Azure Storage to select and run it from the list of applications - then restart the sample.");
                Console.ReadLine();
                throw ;
            }

            return queue;
        }

        public CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
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

        // List
        public void Listqueue()
        {
            CloudStorageAccount storageAccount = CreateStorageAccountFromConnectionString(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            Console.WriteLine("Create a list for queue ");

       
            foreach (var queue in queueClient.ListQueues())
            {
                Console.WriteLine(queue.Uri+"======"+queue.Name);
            }
            
        }



        //get and delete message
        public void ProcessMessageAsyncAll(string queuename)
        {
            Task task = ProcessMessageAsync(queuename);
            for (int i = 0; i <10; i++)
            {
                task = ProcessMessageAsync(queuename);
            }
        }

        public async Task ProcessMessageAsync(string queuename)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the queue client.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue.
            CloudQueue queue = queueClient.GetQueueReference(queuename);

            foreach (CloudQueueMessage message in queue.GetMessages(30 , TimeSpan.FromMinutes(5), null, null))
            {
                // Process all messages in less than 5 minutes, deleting each message after processing.
               await queue.DeleteMessageAsync(message);
               Random r=new Random();
               Console.WriteLine("message delete"+r.Next(0,1000));
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

        /// <summary>
        /// Validate the connection string information in app.config and throws an exception if it looks like 
        /// the user hasn't updated this to valid values. 
        /// </summary>
        /// <param name="storageConnectionString">The storage connection string</param>
        /// <returns>CloudStorageAccount object</returns>
       

        /// <summary>
        /// Delete the queue that was created for this sample
        /// </summary>
        /// <param name="queue">The sample queue to delete</param>
        public async Task DeleteQueueAsync(CloudQueue queue)
        {
            Console.WriteLine("Delete the queue  "+queue.Name);
            await queue.DeleteAsync();
        }
    }
}
