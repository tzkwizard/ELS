using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.Azure;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using ReceiverRole.model;
using ReceiverRole.service;


namespace ReceiverRole
{
    internal class SimpleEventProcessor : IEventProcessor
    {
        private PartitionContext partitionContext;
        private IDBService _iDbService;
        private static IFirebaseClient _client;
        private static DocumentClient _documentClient;
        private static DocumentCollection _documentCollection;
        private static StoredProcedure _sp;
        private static string _endpointUrl;
        private static string _authorizationKey;
        private static string _database;
        private static string _collection;

        public Task OpenAsync(PartitionContext context)
        {
            Init();
            Trace.TraceInformation(string.Format("SimpleEventProcessor OpenAsync.  Partition: '{0}', Offset: '{1}'",
                context.Lease.PartitionId, context.Lease.Offset));
            this.partitionContext = context;
            return Task.FromResult<object>(null);
        }

        private void Init()
        {
            //Get DBservice
            _endpointUrl = _endpointUrl ?? CloudConfigurationManager.GetSetting("DocumentDBUrl");
            _authorizationKey = _authorizationKey ?? CloudConfigurationManager.GetSetting("DocumentDBAuthorizationKey");
            _iDbService = _iDbService ?? new DBService(_endpointUrl, _authorizationKey);

            //Init DB and Firebase
            _database = _database ?? CloudConfigurationManager.GetSetting("Database");
            _collection = _collection ?? CloudConfigurationManager.GetSetting("Collection");
            _client = _client ?? _iDbService.GetFirebaseClient();
            _documentClient = _documentClient ?? _iDbService.GetDocumentClient();
            _documentCollection = _documentCollection ?? _iDbService.GetDc(_documentClient, _collection, _database);
            _sp = _sp ?? _iDbService.GetSp(_documentClient, _documentCollection, "Post");
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> events)
        {
            try
            {
                foreach (EventData eventData in events)
                {
                    try
                    {
                        string dataString = Encoding.UTF8.GetString(eventData.GetBytes());
                        Trace.TraceInformation("Message received.  Partition: '{0}', Data: '{1}', Offset: '{2}'",
                            context.Lease.PartitionId, dataString, eventData.Offset);

                        await SendToDB(dataString);
                    }
                    catch (Exception oops)
                    {
                        Trace.TraceError(oops.Message);
                    }
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
            Trace.TraceWarning(string.Format("SimpleEventProcessor CloseAsync.  Partition '{0}', Reason: '{1}'.",
                this.partitionContext.Lease.PartitionId, reason.ToString()));
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }

        private async Task SendToDB(string dataString)
        {
            dynamic x = JsonConvert.DeserializeObject(dataString);
            //var z = JsonConvert.DeserializeObject<MetricEvent>(dataString);
            var path = x.url.ToString().Split('/');
            x.body.timestamp = (long) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            PostMessage mess = new PostMessage
            {
                Type = "Post",
                Path = new PostPath()
                {
                    District = path[1],
                    School = path[2],
                    Classes = path[3]
                },
                Info = new Info()
                {
                    user = x.body.user,
                    uid = x.body.uid,
                    timestamp = x.body.timestamp,
                    message = x.body.message
                }
            };

            //Send to DB
            var res = await _documentClient.ExecuteStoredProcedureAsync<Document>(
                _sp.SelfLink, mess, _documentCollection.SelfLink);

            //Check response and notify Firebase
            if (res.StatusCode == HttpStatusCode.OK)
            {
                Trace.TraceInformation("DocumentDB received.  Message: '{0}'", res.Response.SelfLink);
                FirebaseResponse response = await _client.PushAsync(x.url.ToString(), x.body);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Trace.TraceInformation("Firebase received.  Message: '{0}'", response.Body);
                }
            }
        }
    }
}