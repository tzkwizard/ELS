using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace doucumentDB
{
    class EventHub
    {
        private static string eventHubName = "eventhub1";

        private static string eventHubConnectionString =
            "Endpoint=sb://azrenweb-ns.servicebus.windows.net/;SharedAccessKeyName=get;SharedAccessKey=+mmaMKj+RjrCUMqC7bK1q4juLrxThN8FKnej026iEus=";

        public static void Receive()
        {
            string storageAccountName = "elsaotuo";
            string storageAccountKey =
                "AV49N0PZ1Qlz42b0w47EPoPbNLULgxYOWxsO4IvFmrAkZPzkdGCKKOJqyiHVGfAPex6HhkDSWpNQAIuPmBHBMA==";
            string storageConnectionString =
                String.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
                    storageAccountName, storageAccountKey);

            string eventProcessorHostName = Guid.NewGuid().ToString();
            EventProcessorHost eventProcessorHost = new EventProcessorHost("1", eventHubName,
                EventHubConsumerGroup.DefaultGroupName, eventHubConnectionString, storageConnectionString);
            Console.WriteLine("Registering EventProcessor...");
            eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>(new EventProcessorOptions()
            {
                InitialOffsetProvider = (partitionId) => DateTime.UtcNow
            }).Wait();


            Console.WriteLine("Receiving. Press enter key to stop worker.");
            Console.ReadLine();
            eventProcessorHost.UnregisterEventProcessorAsync().Wait();
        }
    }
}
