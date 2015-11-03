using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LMS.Common.Models;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace eventHub
{
    class Program
    {
        static string eventHubName = "eventhub1";
        private static string sasKey = "rk8BLb/AQnP7U8AVBpiV9xHGkk2nN5tbxUm2Cx5Vth4=";
        static string eventHubConnectionString = "Endpoint=sb://azrenweb-ns.servicebus.windows.net/;SharedAccessKeyName=get;SharedAccessKey=+mmaMKj+RjrCUMqC7bK1q4juLrxThN8FKnej026iEus=";
        static string eventHubNameSpace = "azrenweb-ns";
        static string eventHubUrl = "https://" + eventHubNameSpace + ".servicebus.windows.net/" + eventHubName + "/messages";
        private static int n = 1;


        static void Main(string[] args)
        {
           //var z = SharedAccessSignatureTokenProvider.GetSharedAccessSignature("post", sasKey , eventHubUrl, TimeSpan.FromHours(1));
           //ReceivePartitionMessage();
            //SendingRandomMessages(1);
         
            BarrierSample();
            Console.ReadLine();
        }
        static void BarrierSample()
        {
            int count = 0;

            // Create a barrier with three participants
            // Provide a post-phase action that will print out certain information
            // And the third time through, it will throw an exception
            Barrier barrier = new Barrier(3, (b) =>
            {
                Console.WriteLine("Post-Phase action: count={0}, phase={1}", count, b.CurrentPhaseNumber);
                Console.WriteLine(b.ParticipantCount);
                //if (b.CurrentPhaseNumber == 2) throw new Exception("D'oh!");
            });

            // Nope -- changed my mind.  Let's make it five participants.
            barrier.AddParticipants(7);

            // Nope -- let's settle on four participants.
            //barrier.RemoveParticipant();


            // This is the logic run by all participants
            Action action = () =>
            {
                Interlocked.Increment(ref count);
                barrier.SignalAndWait(); // during the post-phase action, count should be 4 and phase should be 0
                /*Interlocked.Increment(ref count);
                barrier.SignalAndWait(); // during the post-phase action, count should be 8 and phase should be 1

                // The third time, SignalAndWait() will throw an exception and all participants will see it
                Interlocked.Increment(ref count);
                try
                {
                    barrier.SignalAndWait();
                }
                catch (BarrierPostPhaseException bppe)
                {
                    Console.WriteLine("Caught BarrierPostPhaseException: {0}", bppe.Message);
                }

                // The fourth time should be hunky-dory
                Interlocked.Increment(ref count);
                barrier.SignalAndWait(); // during the post-phase action, count should be 16 and phase should be 3*/
            };

            Action[] z=new Action[10];
            for (int i=0;i<10;i++)
            {
                z[i] = action;
            }
            // Now launch 4 parallel actions to serve as 4 participants
            //Parallel.Invoke(z);

            Parallel.For(1,400, i=>
            {

                SendingRandomMessages(n);
                Interlocked.Increment(ref n);
            });

            // This (5 participants) would cause an exception:
            // Parallel.Invoke(action, action, action, action, action);
            //      "System.InvalidOperationException: The number of threads using the barrier
            //      exceeded the total number of registered participants."

            // It's good form to Dispose() a barrier when you're done with it.
            //barrier.Dispose();

        }
        
  
        private static void ReceivePartitionMessage()
        {
            var eventHubClient = EventHubClient.CreateFromConnectionString(eventHubConnectionString, eventHubName);
            EventHubConsumerGroup group = eventHubClient.GetDefaultConsumerGroup();
            var receiver = group.CreateReceiver(eventHubClient.GetRuntimeInformation().PartitionIds[1]);
            while (true)
            {
                var message = receiver.Receive();
                var myOffset = message.Offset;
                string body = Encoding.UTF8.GetString(message.GetBytes());
                Console.WriteLine(String.Format("Received message offset: {0} \nbody: {1}", myOffset, body));
            }
        }
        private static void SendingRandomMessages(int nu)
        {
            var i = 0;
            var eventHubClient = EventHubClient.CreateFromConnectionString(eventHubConnectionString, eventHubName);
            while (i<1)
            {
                try
                {
                    test t= new test
                    {
                        url="LMS/tst-azhang1/HS/Java",
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
                    Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, message);
                }
                catch (Exception exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("{0} > Exception: {1}", DateTime.Now, exception.Message);
                    Console.ResetColor();
                }
                i++;
                Thread.Sleep(2000);
            }
        }
    }
}
