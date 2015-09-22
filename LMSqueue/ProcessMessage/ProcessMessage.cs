using System;
using System.Net;
using System.Threading.Tasks;
using System.Configuration;
using System.Linq;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using MessageHandleApi.Models;
using Microsoft.WindowsAzure.Storage;
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
        private static string EndpointUrl = "https://tstaz.documents.azure.com:443/";
        private static string AuthorizationKey =
            "6xPkxpC7FyiozobQOtQ8yFxbqd7uLOCz0pRo4i+GKxHdmISxDrMKZdaKQH0/0BJe/xC3UKdQM4C1x5d4Rxk3AQ==";
        private readonly DocumentClient _documentClient;


        public ProcessMessage()
        {
            _documentClient = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
            _client = GetFirebase();
            _queue = GetQueue();
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
                var documentCollection = GetDc(_documentClient, "LMSCollection", "LMSRegistry");
                var sp =
                    _documentClient.CreateStoredProcedureQuery(documentCollection.SelfLink).Where(c => c.Id == "Post")
                        .AsEnumerable()
                        .FirstOrDefault();
                var path = x.url.ToString().Split('/');
                if (sp != null)
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
                        sp.SelfLink, mess, documentCollection.SelfLink);
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

        private static CloudQueue GetQueue()
        {
            var storageAccount =
                CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ToString());
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("queue");
            return queue;
        }

        private static IFirebaseClient GetFirebase()
        {
            var node = "https://dazzling-inferno-4653.firebaseio.com/";
            var firebaseSecret = "F1EIaYtnYgfkVVI7sSBe3WDyUMlz4xV6jOrxIuxO";
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = firebaseSecret,
                BasePath = node
            };
            var client = new FirebaseClient(config);
            return client;
        }

        private static Database GetDd(DocumentClient client, string dName)
        {
            Database database = client.CreateDatabaseQuery().Where(db => db.Id == dName)
                .AsEnumerable().FirstOrDefault();

            return database;
        }

        private static DocumentCollection GetDc(DocumentClient client, string cName, string dName)
        {
            var database = GetDd(client, dName);
            DocumentCollection documentCollection = client.CreateDocumentCollectionQuery(database.SelfLink)
                .Where(c => c.Id == cName)
                .AsEnumerable()
                .FirstOrDefault();

            return documentCollection;
        }


    }
}