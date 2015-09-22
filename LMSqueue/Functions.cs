using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMSqueue.ProcessMessage;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace LMSqueue
{
    public class Functions
    {
        private static IProcessMessage _iProcessMessage;
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        static Functions()
        {
            _iProcessMessage = new ProcessMessage.ProcessMessage();
        }
        public static void ProcessQueueMessage([QueueTrigger("queue")] string message, TextWriter log ,CloudStorageAccount cloudStorageAccount, string queueTrigger, string id, string popReceipt, int dequeueCount,
            DateTimeOffset expirationTime, DateTimeOffset insertionTime, DateTimeOffset nextVisibleTime)
        {
            log.WriteLine(
              "logMessage={0}\n" + "expirationTime={1}\ninsertionTime={2}\n" + "nextVisibleTime={3}\n" +
              "id={4}\npopReceipt={5}\ndequeueCount={6}\n" + "queue endpoint={7} queueTrigger={8}",
              message, expirationTime, insertionTime, nextVisibleTime, id, popReceipt, dequeueCount,
              cloudStorageAccount.QueueEndpoint, queueTrigger);
         
            _iProcessMessage.SendMessageAsync();
            _iProcessMessage.Send(message);
        }

    }
}
