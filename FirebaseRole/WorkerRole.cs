using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace FirebaseRole
{
    public class WorkerRole : RoleEntryPoint
    {
        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        private ChatBackup _chatBackup;
        private CloudQueue _client;
        private ManualResetEvent CompletedEvent = new ManualResetEvent(false);
        private bool _flag;

        public override void Run()
        {
            Trace.WriteLine("Starting processing");

            _flag = true;
            Queue();
            CompletedEvent.WaitOne();
        }

        private void Queue()
        {
            while (_flag)
            {
                CloudQueueMessage q = _client.GetMessage();
                if (q != null && _chatBackup != null)
                {
                    _client.DeleteMessage(q);
                    Trace.TraceInformation("Start Check Collection Usage.  Time: '{0}'",
                        DateTime.Now.ToString(CultureInfo.CurrentCulture));
                    _flag = false;
                    //Task.Run(() => _chatBackup.BackupDocumentChat());
                    _chatBackup.BackupDocumentChat().Wait();
                    Trace.TraceInformation("End Check Collection Usage.  Time: '{0}'",
                        DateTime.Now.ToString(CultureInfo.CurrentCulture));
                    _flag = true;
                }
                Thread.Sleep(1000);
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.AppSettings["AzureStorageConnectionString"]);


            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            _client = queueClient.GetQueueReference("azqueue");

            _client.CreateIfNotExists();

            _chatBackup = new ChatBackup(storageAccount);

            return base.OnStart();
        }

        public override void OnStop()
        {
            _client.Clear();
            _flag = false;
            CompletedEvent.Set();
            base.OnStop();
        }
    }
}