using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.Azure;
using Microsoft.ServiceBus.Messaging;
using ReceiverRole.service;

namespace ReceiverRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private Receiver receiver;
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("ReceiverRole is running");

            //Get settings from configuration
            var eventHubName = CloudConfigurationManager.GetSetting("eventHubName");
            //var consumerGroupName = CloudConfigurationManager.GetSetting("consumerGroupName");
            //var numberOfPartitions = int.Parse(CloudConfigurationManager.GetSetting("numberOfPartitions"));
            var blobConnectionString = CloudConfigurationManager.GetSetting("AzureStorageConnectionString"); // Required for checkpoint/state

            //Get AMQP connection string
            var connectionString = EventHubManager.GetServiceBusConnectionString();

            //Create event hub if it does not exist
           /* var namespaceManager = EventHubManager.GetNamespaceManager(connectionString);
            EventHubManager.CreateEventHubIfNotExists(eventHubName, numberOfPartitions, namespaceManager);*/

            //Create consumer group if it does not exist
            /*var group = namespaceManager.CreateConsumerGroupIfNotExists(eventHubName, consumerGroupName);*/

            //Start processing messages
            receiver = new Receiver(eventHubName, connectionString);
            ConsumerGroupDescription group = null;
            //Get host name of worker role instance.  This is used for each environment to obtain
            //a lease, and to renew the same lease in case of a restart.
            string hostName = RoleEnvironment.CurrentRoleInstance.Id;
            receiver.RegisterEventProcessor(group, blobConnectionString, hostName);

            //Wait for shutdown to be called, else the role will recycle
            this.runCompleteEvent.WaitOne();
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("ReceiverRole has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("ReceiverRole is stopping");

            this.runCompleteEvent.Set();
            try
            {
                //Unregister the event processor so other instances
                //  will handle the partitions
                receiver.UnregisterEventProcessor();
            }
            catch (Exception oops)
            {
                Trace.TraceError(oops.Message);
            }

            base.OnStop();

            Trace.TraceInformation("ReceiverRole has stopped");
        }

    }
}