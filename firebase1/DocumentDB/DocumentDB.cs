using System;
using System.Linq;
using System.Threading.Tasks;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using MessageHandleApi.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;

namespace firebase1.DocumentDB
{
    public class DocumentDB : IDocumentDB
    {
        private static string EndpointUrl = "https://tstaz.documents.azure.com:443/";

        private static string AuthorizationKey =
            "6xPkxpC7FyiozobQOtQ8yFxbqd7uLOCz0pRo4i+GKxHdmISxDrMKZdaKQH0/0BJe/xC3UKdQM4C1x5d4Rxk3AQ==";

        private readonly DocumentClient _client;
        private static IFirebaseClient _firebaseClient;
        private static DocumentCollection _documentCollection;
        private static StoredProcedure _sp;

        public DocumentDB()
        {
            _client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
            _firebaseClient = GetFirebaseClient();
            _documentCollection = GetDc(_client, "LMSCollection", "LMSRegistry");
            _sp =
              _client.CreateStoredProcedureQuery(_documentCollection.SelfLink).Where(c => c.Id == "Post")
                  .AsEnumerable()
                  .FirstOrDefault();
        }

        private static IFirebaseClient GetFirebaseClient()
        {
            var node = "https://dazzling-inferno-4653.firebaseio.com/";
            var firebaseSecret = "F1EIaYtnYgfkVVI7sSBe3WDyUMlz4xV6jOrxIuxO";
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = firebaseSecret,
                BasePath = node
            };
            IFirebaseClient client = new FirebaseClient(config);
            return client;
        }

        public async Task UpdateDocument()
        {
            if (_sp != null)
            {
                await AddDocumentChat();
                await AddDocumentPost();
            }
        }

        private Database GetDd(DocumentClient client, string dName)
        {
            Database database = client.CreateDatabaseQuery().Where(db => db.Id == dName)
                .AsEnumerable().FirstOrDefault();

            return database;
        }

        private DocumentCollection GetDc(DocumentClient client, string cName, string dName)
        {
            var database = GetDd(_client, dName);
            DocumentCollection documentCollection = client.CreateDocumentCollectionQuery(database.SelfLink)
                .Where(c => c.Id == cName)
                .AsEnumerable()
                .FirstOrDefault();

            return documentCollection;
        }

        private async Task AddDocumentChat()
        {
            FirebaseResponse chatResponse = _firebaseClient.Get("ChatRoom");

            dynamic ds = JsonConvert.DeserializeObject(chatResponse.Body);
            try
            {
                foreach (var d in ds)
                {
                    dynamic t = d;
                    //await Task.Run(() => Send(t, documentCollection.SelfLink,sp.SelfLink));
                    await SendChat(t, _documentCollection.SelfLink, _sp.SelfLink);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async Task SendChat(dynamic u, string path, string sp)
        {
            if (u.Value.messages != null && u.Value.messages.Value != "")
            {
                foreach (var s in u.Value.messages)
                {
                    ChatMessage c = new ChatMessage
                    {
                        Type = "Chat",
                        Info = new Info
                        {
                            message = s.Value.message,
                            uid = s.Value.uid,
                            timestamp = s.Value.timestamp,
                            user = s.Value.user
                        },
                        Room = new ChatRoom
                        {
                            Id = u.Value.room.ID,
                            Name = u.Value.room.Name
                        }
                    };
                   await _client.ExecuteStoredProcedureAsync<Document>(
                        sp, c, path);
                }
            }
        }

        private async Task AddDocumentPost()
        {
            FirebaseResponse postResponse = _firebaseClient.Get("LMS");

            dynamic ds = JsonConvert.DeserializeObject(postResponse.Body);
            try
            {
                await SendPost(ds, _documentCollection.SelfLink, _sp.SelfLink);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async Task SendPost(dynamic u, string path, string sp)
        {
            foreach (var d in u)
            {
                foreach (var s in d.Value)
                {
                    foreach (var c in s.Value)
                    {
                        foreach (var p in c.Value)
                        {
                            PostMessage m = new PostMessage
                            {
                                Type = "Post",
                                Path = new PostPath()
                                {
                                    District = d.Name,
                                    School = s.Name,
                                    Classes = c.Name
                                },
                                Info = new Info()
                                {
                                    user = p.Value.user,
                                    uid = p.Value.uid,
                                    timestamp = p.Value.timestamp,
                                    message = p.Value.message
                                }
                            };
                            await _client.ExecuteStoredProcedureAsync<Document>(
                                sp, m, path);
                        }
                    }
                }
            }
        }

    }
}