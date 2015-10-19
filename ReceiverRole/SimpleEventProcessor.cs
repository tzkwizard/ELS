using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FireSharp.Exceptions;
using FireSharp.Interfaces;
using FireSharp.Response;
using LMS.model.Models;
using LMS.service.Service;
using Microsoft.Azure;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;


namespace ReceiverRole
{
    internal class SimpleEventProcessor : IEventProcessor
    {
        private PartitionContext partitionContext;
        private IDBService _iDbService;
        private static IFirebaseClient _client;
        private static DocumentClient _documentClient;
        private static Database _database;
        private static string _databaseName;

        public Task OpenAsync(PartitionContext context)
        {
            Init();
            Trace.TraceInformation("SimpleEventProcessor OpenAsync.  Partition: '{0}', Offset: '{1}'",
                context.Lease.PartitionId, context.Lease.Offset);
            this.partitionContext = context;
            return Task.FromResult<object>(null);
        }

        private void Init()
        {
            //Get DBservice
            _iDbService = _iDbService ?? new DBService();

            //Init DB and Firebase
            _databaseName = _databaseName ?? CloudConfigurationManager.GetSetting("Database");
            _client = _client ?? _iDbService.GetFirebaseClient();
            _documentClient = _documentClient ?? _iDbService.GetDocumentClient();
            _database = _database ?? _iDbService.GetDd(_databaseName);
            _documentClient.OpenAsync();
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> events)
        {
            try
            {
                CheckResolver();
                foreach (EventData eventData in events)
                {
                    string dataString = Encoding.UTF8.GetString(eventData.GetBytes());
                    Trace.TraceInformation("Message received.  Partition: '{0}', Data: '{1}', Offset: '{2}'",
                        context.Lease.PartitionId, dataString, eventData.Offset);

                    await SendToDB(dataString);
                }
                await context.CheckpointAsync();
            }
            catch (Exception exp)
            {
                Trace.TraceError("Error in processing: " + exp.Message);
            }
        }

        public async Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Trace.TraceWarning("SimpleEventProcessor CloseAsync.  Partition '{0}', Reason: '{1}'.",
                this.partitionContext.Lease.PartitionId, reason.ToString());
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }

        private void CheckResolver()
        {
            var resolver = _iDbService.GetResolver();

            _documentClient.PartitionResolvers[_database.SelfLink] = resolver;
        }

        private async Task SendToDB(string dataString)
        {
            dynamic data = JsonConvert.DeserializeObject(dataString);
            var path = data.url.ToString().Split('/');
            data.body.timestamp = (long) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            PostMessage message = _iDbService.PostData(data, path);

            //Send to DB         
            //create document with RangePartitionResolver
            var res =
                await
                    _iDbService.ExecuteWithRetries(
                        () => _documentClient.CreateDocumentAsync(_database.SelfLink, message));

            //Check response and notify Firebase
            if (res.StatusCode == HttpStatusCode.Created)
            {
                Trace.TraceInformation("DocumentDB received. Message: '{0}'", res.Resource.SelfLink);
                try
                {
                    FirebaseResponse response = await _client.PushAsync(data.url.ToString(), data.body);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Trace.TraceInformation("Firebase received.  Message: '{0}'", response.Body);
                    }
                }
                catch (FirebaseException e)
                {
                    Trace.TraceError("Error in push to Firebase: " + e.Message);
                }
            }
        }
    }
}