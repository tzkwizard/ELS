using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FireSharp.Exceptions;
using FireSharp.Interfaces;
using FireSharp.Response;
using LMS.Common.Models;
using LMS.Common.Service;
using LMS.Common.Service.Interface;
using Microsoft.Azure;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Partitioning;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Newtonsoft.Json;

namespace MessageHandleApi.Test
{
    internal class EventProcessor : IEventProcessor
    {
        private PartitionContext partitionContext;
        private IDbService _iDbService;
        private static IFirebaseClient _client;
        private static string _databaseSelfLink;

        public Task OpenAsync(PartitionContext context)
        {
            Init();
            this.partitionContext = context;
            return Task.FromResult<object>(null);
        }

        private void Init()
        {
            //Get DBservice
            _databaseSelfLink = _databaseSelfLink ?? ConfigurationManager.AppSettings["DBSelfLink"];
            _iDbService = _iDbService ?? new DbService();
            //Init DB and Firebase
            _client = _client ?? _iDbService.GetFirebaseClient();
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> events)
        {
            try
            {
                var client = await GetDocumentClient();
                foreach (EventData eventData in events)
                {
                    string dataString = Encoding.UTF8.GetString(eventData.GetBytes());
                    ConfigurationManager.AppSettings["LMS1"] = "true";
                    await SendToDb(client, dataString);
                }

                await context.CheckpointAsync();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Error in processing: " + exp.Message);
            }
        }

        public async Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }

        private async Task<DocumentClient> GetDocumentClient()
        {
            _iDbService.UpdateDocumentClient();
            var client = _iDbService.GetDocumentClient();
            while (client.PartitionResolvers.Count == 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                client = _iDbService.GetDocumentClient();
            }
            return client;
        }

        private async Task SendToDb(DocumentClient client, string dataString)
        {
            dynamic data = JsonConvert.DeserializeObject(dataString);
            var path = data.url.ToString().Split('/');
            data.body.timestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            PostMessage message = ModelService.PostData(data, path);

            //create document on DB with RangePartitionResolver
            var res =
                await
                    RetryService.ExecuteWithRetries(
                        () => client.CreateDocumentAsync(_databaseSelfLink, message));
            //Check response and notify Firebase
            if (res.StatusCode == HttpStatusCode.Created)
            {
                ConfigurationManager.AppSettings["LMS2"] = "true";               
                try
                {
                    FirebaseResponse response = await _client.PushAsync(data.url.ToString(), data.body);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        ConfigurationManager.AppSettings["LMS3"] = "true";
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