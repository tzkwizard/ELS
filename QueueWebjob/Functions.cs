using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Data.OData.Query.SemanticAst;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Ninject;
using QueueWebjob.QueueAndRedis;
using StackExchange.Redis;


namespace QueueWebjob
{
    public class Functions
    {
        private static IQueueAndredis _iqueueandredis;
        private static CloudQueue _queue;
        public Functions(IQueueAndredis iqueueandredis)
        {

            _iqueueandredis = iqueueandredis;

        }



        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([QueueTrigger("elsqueue")] string message, TextWriter log,
            CloudStorageAccount cloudStorageAccount, string queueTrigger, string id, string popReceipt, int dequeueCount,
            DateTimeOffset expirationTime, DateTimeOffset insertionTime, DateTimeOffset nextVisibleTime)
        {
            // Redis();
            //Writing log on webjob dashboard
            log.WriteLine(
                "logMessage={0}\n" + "expirationTime={1}\ninsertionTime={2}\n" + "nextVisibleTime={3}\n" +
                "id={4}\npopReceipt={5}\ndequeueCount={6}\n" + "queue endpoint={7} queueTrigger={8}",
                message, expirationTime, insertionTime, nextVisibleTime, id, popReceipt, dequeueCount,
                cloudStorageAccount.QueueEndpoint, queueTrigger);

            Console.WriteLine("From: " + cloudStorageAccount.QueueEndpoint);


            if (_iqueueandredis == null)
            {
                var kernel = new StandardKernel();

                kernel.Bind<IQueueAndredis>().To<QueueAndredis>();
                _iqueueandredis = kernel.Get<QueueAndredis>();
            }

            //Task.Run(() => q.ProcessAsync("qqwwss")); 
            if (_queue == null)
            {
                _queue = _iqueueandredis.GetQueue();
            }

            _iqueueandredis.InsertData(message);
            _iqueueandredis.ProcessMessageAsync(_queue);
        }

        // This function will get triggered/executed when poison-message(message fail to function will become poison) is written. 
        public static void ProcessPoisonMessage(
        [QueueTrigger("elsqueue-poison")] string blobName, TextWriter logger)
        {
            logger.WriteLine("Failed to copy blob, name=" + blobName);
        }




        // dummy function for ProcessQueueMessage, only used for test
        public void ProcessQueueMessage(String message)
        {
            //IQueueAndredis iqueueandredis = new QueueAndredis();
            if (_queue == null)
            {
                _queue = _iqueueandredis.GetQueue();
            }
            _iqueueandredis.InsertData(message);
            _iqueueandredis.ProcessMessageAsync(_queue);
        }


        public static void Redis()
        {

            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect("elsazure.redis.cache.windows.net,password=JL2ATsoHzQ8PHw4LSj/VgpgcEPlCkKKT3Qfvr69yrCA=,allowAdmin=true");
            IDatabase cache = connection.GetDatabase();
            var server = connection.GetServer("elsazure.redis.cache.windows.net:6379");
            server.FlushDatabase();

            /*   string tem="::1 - - [10/Feb/2016:15:49:12 -06] "+"\"POST /api/contact\""+" 200 2 "+"\"http://localhost:59524/\""+
                  " \"Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.111 Safari/537.36\"";*/
            cache.ListLeftPush("s", "123");
            foreach (var s in cache.ListRange("s"))
            {
                Console.WriteLine(s);
            }

            foreach (var s in connection.GetEndPoints())
            {
                Console.WriteLine(s);
            }
            // Simple get of data types from the cache
            /* foreach (var i in server.Keys())
             {               
                 string key1 = cache.StringGet(i);
                 int key2 = (int) cache.StringGet("key2");
                 Console.WriteLine(key1 + "" + key2);
             }*/
        }


    }
}
