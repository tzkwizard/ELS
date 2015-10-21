using System.Diagnostics;
using Microsoft.ServiceBus.Messaging;

namespace EventRole
{
    class Receiver
    {
        string eventHubName;
        string eventHubConnectionString;
        EventProcessorHost eventProcessorHost;
        public Receiver(string eventHubName, string eventHubConnectionString)
        {
            this.eventHubConnectionString = eventHubConnectionString;
            this.eventHubName = eventHubName;
        }

        public void RegisterEventProcessor(ConsumerGroupDescription group, string blobConnectionString, string hostName)
        {
            EventHubClient eventHubClient = EventHubClient.CreateFromConnectionString(eventHubConnectionString, this.eventHubName);

            if (null == group)
            {
                //Use default consumer group
                EventHubConsumerGroup defaultConsumerGroup = eventHubClient.GetDefaultConsumerGroup();
                eventProcessorHost = new EventProcessorHost(hostName, eventHubClient.Path, defaultConsumerGroup.GroupName, this.eventHubConnectionString, blobConnectionString);
            }
            else
            {
                //Use custom consumer group
                eventProcessorHost = new EventProcessorHost(hostName, eventHubClient.Path, group.Name, this.eventHubConnectionString, blobConnectionString);
            }


            Trace.TraceInformation("Registering event processor");

            eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>().Wait();
        }

        public void UnregisterEventProcessor()
        {
            eventProcessorHost.UnregisterEventProcessorAsync().Wait();
        }
    }
}
