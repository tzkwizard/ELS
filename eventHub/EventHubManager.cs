using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.Azure;

namespace eventHub
{
    public class EventHubManager
    {
        public static string GetServiceBusConnectionString()
        {
            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");

            if (string.IsNullOrEmpty(connectionString))
            {
                Trace.WriteLine("Did not find Service Bus connections string in appsettings (app.config)");
                return string.Empty;
            }
            ServiceBusConnectionStringBuilder builder = new ServiceBusConnectionStringBuilder(connectionString);
            builder.TransportType = TransportType.Amqp;
            return builder.ToString();
        }

        public static NamespaceManager GetNamespaceManager()
        {
            return NamespaceManager.CreateFromConnectionString(GetServiceBusConnectionString());
        }

        public static NamespaceManager GetNamespaceManager(string connectionString)
        {
            return NamespaceManager.CreateFromConnectionString(connectionString);
        }


        public static void CreateEventHubIfNotExists(string eventHubName, int numberOfPartitions, NamespaceManager manager)
        {
            try
            {
                // Create the Event Hub
                Trace.WriteLine("Creating Event Hub...");
                EventHubDescription ehd = new EventHubDescription(eventHubName);
                ehd.PartitionCount = numberOfPartitions;
                manager.CreateEventHubIfNotExistsAsync(ehd).Wait();
            }
            catch (AggregateException agexp)
            {
                Trace.WriteLine(agexp.Flatten());
            }
        }

    }
    
}
