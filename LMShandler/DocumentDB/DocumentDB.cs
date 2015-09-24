using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FireSharp.Interfaces;
using FireSharp.Response;
using MessageHandleApi.Models;
using MessageHandleApi.Service;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;

namespace LMShandler.DocumentDB
{
    public class DocumentDB : IDocumentDB
    {
        private readonly DocumentClient _client;
        private static IFirebaseClient _firebaseClient;
        private static DocumentCollection _documentCollection;
        private static StoredProcedure _sp;
        private readonly long _start;
        private readonly long _end;
        private const int RecordRemained = 10;

        public DocumentDB(IDBService iDbService)
        {
            _client = iDbService.GetDocumentClient();
            _firebaseClient = iDbService.GetFirebaseClient();
            _documentCollection = iDbService.GetDc(_client, "LMSCollection", "LMSRegistry");
            _sp = iDbService.GetSp(_client, _documentCollection, "Post");
            _end = (long) (DateTime.UtcNow.AddMinutes(-15).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            _start = (long) (DateTime.UtcNow.AddHours(-25).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
        }

        public async Task UpdateDocument()
        {
            if (_sp != null)
            {
                await BackupDocumentChat();
                await CleanDocumentPost();
                //Console.ReadLine();
            }
        }

        private async Task BackupDocumentChat()
        {
            FirebaseResponse chatResponse = _firebaseClient.Get("ChatRoom");
            dynamic ds = JsonConvert.DeserializeObject(chatResponse.Body);

            foreach (var d in ds)
            {
                dynamic t = d;
                //await Task.Run(() => Send(t, documentCollection.SelfLink,sp.SelfLink));
                await Backup(t, _documentCollection.SelfLink, _sp.SelfLink);
            }
        }

        private async Task Backup(dynamic u, string path, string sp)
        {
            try
            {
                if (u.Value.messages.Value != "")
                {
                    var origin = new List<dynamic>();
                    foreach (var s in u.Value.messages)
                    {
                        origin.Add(s);
                    }

                    var sorted = origin.OrderBy(o => o.Value.timestamp).ToList();

                    if (sorted.Count <= RecordRemained) return;

                    for (int i = 0; i < sorted.Count - RecordRemained; i++)
                    {
                        var s = sorted[i];
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
                        var res = await _client.ExecuteStoredProcedureAsync<Document>(
                            sp, c, path);
                        if (res.StatusCode == HttpStatusCode.OK)
                        {
                            string url = "ChatRoom/" + u.Name + "/messages/" + s.Name;
                            await _firebaseClient.DeleteAsync(url);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private async Task CleanDocumentPost()
        {
            FirebaseResponse postResponse = _firebaseClient.Get("LMS");
            dynamic ds = JsonConvert.DeserializeObject(postResponse.Body);
            foreach (var d in ds)
            {
                await Clean(d, _documentCollection.SelfLink, _sp.SelfLink);
            }
        }

        private async Task Clean(dynamic d, string path, string sp)
        {
            try
            {
                foreach (var s in d.Value)
                {
                    foreach (var c in s.Value)
                    {
                        var origin = new List<dynamic>();
                        foreach (var p in c.Value)
                        {
                            origin.Add(p);
                        }

                        var sorted = origin.OrderBy(o => o.Value.timestamp).ToList();

                        for (int i = 0; i < sorted.Count - 1; i++)
                        {
                            var sort = sorted[i];
                            string url = "LMS/" + d.Name + "/" + s.Name + "/" + c.Name + "/" + sort.Name;
                            await _firebaseClient.DeleteAsync(url);
                        }
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