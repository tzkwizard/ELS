using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace StorageRole
{
    public class WorkerRole : RoleEntryPoint
    {

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        private PostBackup _postBackup;
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
                if (q != null && _postBackup != null)
                {
                    _client.DeleteMessage(q);
                    Trace.TraceInformation("Start Backup Post.  Time: '{0}'",
                        DateTime.Now.ToString(CultureInfo.CurrentCulture));
                    _flag = false;
                    _postBackup.BackupPostAll().Wait();
                    Trace.TraceInformation("End Backup Post.  Time: '{0}'",
                        DateTime.Now.ToString(CultureInfo.CurrentCulture));
                    //_postBackup.CleanCollection().Wait();
                    Trace.TraceInformation("End Clean Collection.  Time: '{0}'",
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
                CloudConfigurationManager.GetSetting("Microsoft.Storage.ConnectionString"));

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            _client = queueClient.GetQueueReference("azqueue");

            _client.CreateIfNotExists();
            var url = CloudConfigurationManager.GetSetting("EndpointUrl");
            var key = CloudConfigurationManager.GetSetting("AuthorizationKey");

            _postBackup = new PostBackup(storageAccount, url, key);

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
