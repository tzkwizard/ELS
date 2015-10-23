using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace CheckRole
{
    public class WorkerRole : RoleEntryPoint
    {
        // The name of your queue
        private const string QueueName = "ProcessingQueue";

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        private CloudQueue _client;
        private Dichotomy _dichotomy;
        private LoadBalance _loadBalance;
        private ManualResetEvent CompletedEvent = new ManualResetEvent(false);
        private bool _flag;
        private static string _database;

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
                if (q != null && _dichotomy != null)
                {
                    _client.DeleteMessage(q);
                    Trace.TraceInformation("Start Check Collection Usage.  Time: '{0}'",
                        DateTime.Now.ToString(CultureInfo.CurrentCulture));
                    _flag = false;
                    //Task.Run(() => _dichotomy.UpdateDcAll());
                    //_dichotomy.UpdateDcAll().Wait();
                    _loadBalance.CheckBalance(_database).Wait();
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
                CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString"));

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            _client = queueClient.GetQueueReference("azqueue");

            _client.CreateIfNotExists();
            _database = CloudConfigurationManager.GetSetting("DBSelfLink");
            var url = CloudConfigurationManager.GetSetting("DocumentDBUrl");
            var key = CloudConfigurationManager.GetSetting("DocumentDBAuthorizationKey");
            _dichotomy = new Dichotomy(url, key);
            _loadBalance = new LoadBalance(url, key);
            return base.OnStart();
        }

        public override void OnStop()
        {
            _flag = false;
            _client.Clear();
            CompletedEvent.Set();
            base.OnStop();
        }
    }
}