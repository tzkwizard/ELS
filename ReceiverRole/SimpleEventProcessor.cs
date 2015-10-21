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
using Microsoft.Azure.Documents.Partitioning;


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
        private static string _endpointUrl;
        private static string _authorizationKey;
        private static RangePartitionResolver<long> _resolver;

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
            _client = _client ?? _iDbService.GetFirebaseClient();
            _documentClient = _documentClient ?? _iDbService.GetDocumentClient();
            _database = _database ?? _iDbService.GetDd(_databaseName);
            _documentClient.OpenAsync();
            Task.Run(() => CheckResolver());
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

        private async Task CheckResolver()
        {
            while (true)
            {
                if (_documentClient.PartitionResolvers.Count == 0 && _resolver.PartitionMap.Count != 0)
                {
                    _documentClient.PartitionResolvers[_database.SelfLink] = _resolver;
                }
                else
                {
                    var resolver = _iDbService.GetResolver();
                    if (_resolver.PartitionMap.Count != resolver.PartitionMap.Count)
                    {
                        _documentClient.PartitionResolvers[_database.SelfLink] = resolver;
                        _resolver = resolver;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(10));
                }
            }
        }

        private async Task SendToDB(string dataString)
        {
            dynamic data = JsonConvert.DeserializeObject(dataString);
            var path = data.url.ToString().Split('/');
            data.body.timestamp = (long) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            PostMessage message = _iDbService.PostData(data, path);

            //create document on DB with RangePartitionResolver
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