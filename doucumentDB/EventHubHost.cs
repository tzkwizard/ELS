using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using System.Diagnostics;
using FireSharp.Interfaces;
using FireSharp.Response;
using LMS.Common.Models;
using LMS.Common.Service;
using Microsoft.ServiceBus;


namespace doucumentDB
{
    public class SimpleEventProcessor : IEventProcessor
    {
        private Stopwatch checkpointStopWatch;
        private static IFirebaseClient _client;

        async Task IEventProcessor.CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine("Processor Shutting Down. Partition '{0}', Reason: '{1}'.", context.Lease.PartitionId,
                reason);
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }

        Task IEventProcessor.OpenAsync(PartitionContext context)
        {
            Console.WriteLine("SimpleEventProcessor initialized.  Partition: '{0}', Offset: '{1}'",
                context.Lease.PartitionId, context.Lease.Offset);
            this.checkpointStopWatch = new Stopwatch();
            this.checkpointStopWatch.Start();
            return Task.FromResult<object>(null);
        }


        async Task IEventProcessor.ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            var iDbService = new DbService();
            _client = iDbService.GetFirebaseClient();
            foreach (EventData eventData in messages)
            {
                string data = Encoding.UTF8.GetString(eventData.GetBytes());
                FirebaseResponse response = await _client.PushAsync("event", new EHdata
                {
                    offset = eventData.Offset,
                    body = data,
                    partitionId = context.Lease.PartitionId
                });
                Console.WriteLine(String.Format("Message received.  Partition: '{0}', Data: '{1}', Offset: '{2}'",
                    context.Lease.PartitionId, data, eventData.Offset));
            }

            //Call checkpoint every 5 minutes, so that worker can resume processing from the 5 minutes back if it restarts.
            if (this.checkpointStopWatch.Elapsed > TimeSpan.FromMinutes(5))
            {
                Console.WriteLine(this.checkpointStopWatch.Elapsed);
                await context.CheckpointAsync();
                this.checkpointStopWatch.Restart();
            }
        }
    }
}