using System.Diagnostics;
using Microsoft.ServiceBus.Messaging;

namespace EventRole
{
    internal class Receiver
    {
        private string eventHubName;
        private string eventHubConnectionString;
        private EventProcessorHost eventProcessorHost;

        public Receiver(string eventHubName, string eventHubConnectionString)
        {
            this.eventHubConnectionString = eventHubConnectionString;
            this.eventHubName = eventHubName;
        }

        public void RegisterEventProcessor(ConsumerGroupDescription group, string blobConnectionString, string hostName)
        {
            EventHubClient eventHubClient = EventHubClient.CreateFromConnectionString(eventHubConnectionString,
                this.eventHubName);

            if (null == group)
            {
                //Use default consumer group
                EventHubConsumerGroup defaultConsumerGroup = eventHubClient.GetDefaultConsumerGroup();
                eventProcessorHost = new EventProcessorHost(hostName, eventHubClient.Path,
                    defaultConsumerGroup.GroupName, this.eventHubConnectionString, blobConnectionString);
            }
            else
            {
                //Use custom consumer group
                eventProcessorHost = new EventProcessorHost(hostName, eventHubClient.Path, group.Name,
                    this.eventHubConnectionString, blobConnectionString);
            }


            Trace.TraceInformation("Registering event processor");

            eventProcessorHost.RegisterEventProcessorAsync<EventProcessor>().Wait();
        }


        public void RegisterEventProcessor(string blobConnectionString, string hostName)
        {
            EventHubClient eventHubClient = EventHubClient.CreateFromConnectionString(eventHubConnectionString,
                this.eventHubName);
            Trace.TraceInformation("register string :" + eventHubConnectionString+ blobConnectionString);
            //Use custom consumer group
            eventProcessorHost = new EventProcessorHost(hostName, eventHubClient.Path,
                EventHubConsumerGroup.DefaultGroupName, this.eventHubConnectionString, blobConnectionString);
            Trace.TraceInformation("Registering event processor");
            eventProcessorHost.RegisterEventProcessorAsync<EventProcessor>().Wait();
        }

        public void UnregisterEventProcessor()
        {
            eventProcessorHost.UnregisterEventProcessorAsync().Wait();
        }
    }
}