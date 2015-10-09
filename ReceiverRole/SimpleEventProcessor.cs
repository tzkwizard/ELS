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
        private static DocumentCollection _masterCollection;
        private static Database _database;
        //private static StoredProcedure _sp;
        private static string _endpointUrl;
        private static string _authorizationKey;
        private static string _databaseName;
        private static string _masterCollectionName;
        private static int _tryTimes = 5;

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
            _endpointUrl = _endpointUrl ?? CloudConfigurationManager.GetSetting("DocumentDBUrl");
            _authorizationKey = _authorizationKey ?? CloudConfigurationManager.GetSetting("DocumentDBAuthorizationKey");
            _iDbService = _iDbService ?? new DBService(_endpointUrl, _authorizationKey);

            //Init DB and Firebase
            _databaseName = _databaseName ?? CloudConfigurationManager.GetSetting("Database");
            _masterCollectionName = _masterCollectionName ?? CloudConfigurationManager.GetSetting("Collection");
            _client = _client ?? _iDbService.GetFirebaseClient();
            _documentClient = _documentClient ?? _iDbService.GetDocumentClient();
            _database = _database ?? _iDbService.GetDd(_documentClient, _databaseName);
            _masterCollection = _masterCollection ??
                                _iDbService.GetDc(_documentClient, _masterCollectionName, _databaseName);
            //_sp = _sp ?? _iDbService.GetSp(_documentClient, _masterCollection, "Post");
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> events)
        {
            try
            {
                foreach (EventData eventData in events)
                {
                    string dataString = Encoding.UTF8.GetString(eventData.GetBytes());
                    Trace.TraceInformation("Message received.  Partition: '{0}', Data: '{1}', Offset: '{2}'",
                        context.Lease.PartitionId, dataString, eventData.Offset);

                    await SendToDB(dataString, _tryTimes);
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

        private async Task SendToDB(string dataString, int tryTimes)
        {
            try
            {
                dynamic data = JsonConvert.DeserializeObject(dataString);
                var path = data.url.ToString().Split('/');
                data.body.timestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
                PostMessage message = _iDbService.PostData(data, path);

                DocumentCollection documentCollection = _iDbService.SearchCollection(path[1], _masterCollection,
                    _database);

                //Send to DB         
                /*var res2 = await _documentClient.ExecuteStoredProcedureAsync<Document>(
                    _sp.SelfLink, mess, _masterCollection.SelfLink);*/
                var res = await _documentClient.CreateDocumentAsync(documentCollection.SelfLink, message);

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
                    catch (Exception e)
                    {
                        Trace.TraceError("Error in push to Firebase: " + e.Message);
                    }
                }
            }
            catch (Exception e)
            {
                //wait and retry 5 times to create document if collection request rate is large
                if (tryTimes > 0)
                {
                    tryTimes--;
                    Thread.Sleep(1000);
                    SendToDB(dataString, tryTimes).Wait();
                }
                else
                {
                    Trace.TraceError("Error in push to DocumentDB: " + e.Message);
                }
            }
        }
    }
}