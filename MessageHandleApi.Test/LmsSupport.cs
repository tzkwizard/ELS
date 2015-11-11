using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LMS.Common.Models;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace MessageHandleApi.Test
{
    public class LmsSupport
    {
        static string eventHubName = "eventhub1";
        private static string sasKey = "rk8BLb/AQnP7U8AVBpiV9xHGkk2nN5tbxUm2Cx5Vth4=";
        static string eventHubConnectionString = "Endpoint=sb://azrenweb-ns.servicebus.windows.net/;SharedAccessKeyName=get;SharedAccessKey=+mmaMKj+RjrCUMqC7bK1q4juLrxThN8FKnej026iEus=";
        static string eventHubNameSpace = "azrenweb-ns";
        static string eventHubUrl = "https://" + eventHubNameSpace + ".servicebus.windows.net/" + eventHubName + "/messages";

        public static void SendingRandomMessages(int n)
        {
            var i = 0;
            var eventHubClient = EventHubClient.CreateFromConnectionString(eventHubConnectionString, eventHubName);
            while (i < n)
            {
                try
                {
                    test t = new test
                    {
                        url = "LMS/tst-azhang1/HS/Java",
                        body = new body
                        {
                            uid = "1201818",
                            timestamp = "1443721768393",
                            message = "haha",
                            user = "Tzkwizard"
                        }
                    };
                    var message = JsonConvert.SerializeObject(t);
                    eventHubClient.Send(new EventData(Encoding.UTF8.GetBytes(message)));
                    Debug.WriteLine("Sent message");
                }
                catch (Exception exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Debug.WriteLine("{0} > Exception: {1}", DateTime.Now, exception.Message);
                    Console.ResetColor();
                }
                i++;
                Thread.Sleep(2000);
            }
        }

        public static void ReceivePartitionMessage()
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
            eventProcessorHost.RegisterEventProcessorAsync<EventProcessor>(new EventProcessorOptions()
            {
                //InitialOffsetProvider = (partitionId) => DateTime.UtcNow
            }).Wait();
            eventProcessorHost.UnregisterEventProcessorAsync().Wait();
        }


    }
}
