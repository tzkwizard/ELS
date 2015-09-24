using System;
using System.Net;
using System.Threading.Tasks;
using System.Configuration;
using System.Linq;
using FireSharp.Interfaces;
using FireSharp.Response;
using MessageHandleApi.Models;
using MessageHandleApi.Service;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace LMSqueue.ProcessMessage
{
    public class ProcessMessage : IProcessMessage
    {
        private static CloudQueue _queue;
        private static IFirebaseClient _client;
        private readonly DocumentClient _documentClient;
        private static DocumentCollection _documentCollection;
        private static StoredProcedure _sp;
        public ProcessMessage(DBService iDbService, QueueService iQueueService)
        {
            _documentClient = iDbService.GetDocumentClient();
            _client = iDbService.GetFirebaseClient();
            _documentCollection = iDbService.GetDc(_documentClient, "LMSCollection", "LMSRegistry");
            _sp = iDbService.GetSp(_documentClient, _documentCollection, "Post");
            _queue = iQueueService.GetQueue("queue");
        }


        public async Task SendMessageAsync()
        {
            foreach (CloudQueueMessage message in _queue.GetMessages(20, TimeSpan.FromSeconds(30), null, null))
            {
                // Process all messages in less than 30s, deleting each message after processing.
                string m = message.AsString;
                await Send(m);
                _queue.DeleteMessage(message);
            }
        }

        public async Task Send(string m)
        {
            try
            {
                dynamic x = JsonConvert.DeserializeObject(m);
                var path = x.url.ToString().Split('/');
                if (_sp != null)
                {
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
                    var res = await _documentClient.ExecuteStoredProcedureAsync<Document>(
                        _sp.SelfLink, mess, _documentCollection.SelfLink);

                    //After save in DB success, save in Firebase
                    if (res.StatusCode == HttpStatusCode.OK)
                    {
                        FirebaseResponse response = await _client.PushAsync(x.url.ToString(), x.body);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

    }
}