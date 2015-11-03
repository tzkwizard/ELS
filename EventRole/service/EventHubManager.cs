﻿using System;
using System.Configuration;
using System.Diagnostics;
using Microsoft.Azure;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;

namespace EventRole.service
{
    public class EventHubManager
    {
        public static string GetServiceBusConnectionString()
        {
            //string connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            //string connectionString = ConfigurationManager.ConnectionStrings["ServiceBusConnectionString"].ToString();
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
                EventHubDescription ehd = new EventHubDescription(eventHubName)
                {
                    PartitionCount = numberOfPartitions
                };
                manager.CreateEventHubIfNotExistsAsync(ehd).Wait();
            }
            catch (AggregateException agexp)
            {
                Trace.WriteLine(agexp.Flatten());
            }
        }

    }
    
}
