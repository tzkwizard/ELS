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
using Microsoft.ApplicationServer.Caching;
using Microsoft.Azure.Documents.Partitioning;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace EventRole
{
    internal class EventProcessor : IEventProcessor
    {
        private PartitionContext partitionContext;
        private IDbService _iDbService;
        private static IFirebaseClient _client;
        private static string _databaseSelfLink;
        private static string _endpointUrl;
        private static string _authorizationKey;
        private static RangePartitionResolver<long> _resolver;
        private static int n = 1;
        private static readonly object _object = new object();
        private static bool _run = false;
        private static bool _start = false;

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
            _run = true;
            //Get DBservice
            _endpointUrl = _endpointUrl ?? CloudConfigurationManager.GetSetting("DocumentDBUrl");
            _authorizationKey = _authorizationKey ?? CloudConfigurationManager.GetSetting("DocumentDBAuthorizationKey");
            _databaseSelfLink = _databaseSelfLink ?? CloudConfigurationManager.GetSetting("DBSelfLink");
            _iDbService = _iDbService ?? new DbService(_endpointUrl, _authorizationKey);

            //Init DB and Firebase
            _client = _client ?? _iDbService.GetFirebaseClient();

            //check resolver state
            lock (_object)
            {
                if (n <= 0) return;
                n--;
                Task.Run(() => CheckResolver());
            }
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> events)
        {
            try
            {
                var client = await GetDocumentClient();
                foreach (EventData eventData in events)
                {
                    string dataString = Encoding.UTF8.GetString(eventData.GetBytes());
                    Trace.TraceInformation("Message received.  Partition: '{0}', Data: '{1}', Offset: '{2}'",
                        context.Lease.PartitionId, dataString, eventData.Offset);

                    await SendToDb(client, dataString);
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
            _run = false;
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }

        private async Task<DocumentClient> GetDocumentClient()
        {
            var client = _iDbService.GetDocumentClient();
            while (client.PartitionResolvers.Count == 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                client = _iDbService.GetDocumentClient();
            }
            return client;
        }

        private async Task CheckResolver()
        {
            _iDbService.UpdateDocumentClient();
            while (_run)
            {
                var client = _iDbService.GetDocumentClient();
                var resolver = _iDbService.GetResolver(client);
                if (_resolver.PartitionMap.Count != resolver.PartitionMap.Count)
                {
                    _iDbService.UpdateDocumentClient();
                    _resolver = resolver;
                }

                var curdc = _resolver.PartitionMap.LastOrDefault().Value;
                var res = await client.ReadDocumentCollectionAsync(curdc);
                var rate = res.CollectionSizeUsage/res.CollectionSizeQuota;

                if (rate > 0.8)
                {
                    await Task.Delay(TimeSpan.FromSeconds(10));
                }
                else
                {
                    await Task.Delay(TimeSpan.FromDays(1));
                }
            }
        }

        private async Task SendToDb(DocumentClient client, string dataString)
        {
            dynamic data = JsonConvert.DeserializeObject(dataString);
            var path = data.url.ToString().Split('/');
            data.body.timestamp = (long) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            PostMessage message = _iDbService.PostData(data, path);

            //create document on DB with RangePartitionResolver
            var res =
                await
                    _iDbService.ExecuteWithRetries(
                        () => client.CreateDocumentAsync(_databaseSelfLink, message));

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