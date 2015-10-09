using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using FireSharp.Interfaces;
using FireSharp.Response;
using LMS.model.Models;
using LMS.service.Service;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace FirebaseRole
{
    public class ChatBackup
    {
        private static IFirebaseClient _firebaseClient;
        private const int RecordRemained = 2;
        private static CloudTable _table;
        private static int reTry = 5;
        private static IDBService _iDbService;

        public ChatBackup(CloudStorageAccount storageAccount)
        {
            CloudTableClient c = storageAccount.CreateCloudTableClient();
            _table = c.GetTableReference("Chat");
            _table.CreateIfNotExists();
            _iDbService = new DBService();
            _firebaseClient = _iDbService.GetFirebaseClient(CloudConfigurationManager.GetSetting("Firebasenode"));
        }

        public async Task BackupDocumentChat()
        {
            try
            {
                FirebaseResponse chatResponse = _firebaseClient.Get("ChatRoom");
                dynamic rooms = JsonConvert.DeserializeObject(chatResponse.Body);

                foreach (var room in rooms)
                {
                    dynamic r = room;
                    await Backup(r, _table, 5);
                }
            }
            catch (Exception e)
            {
                //wait and retry 5 times
                if (reTry > 0)
                {
                    reTry--;
                    Trace.TraceInformation("Restart BackupFirebaseChat... in 1 min " + reTry);
                    Thread.Sleep(60000);
                    BackupDocumentChat().Wait();
                }
                else
                {
                    Trace.TraceError("Error in BackupChat " + e.Message);
                }
            }
        }

        private async Task Backup(dynamic room, CloudTable table, int retry)
        {
            try
            {
                if (room.Value.messages.Value != "")
                {
                    var origin = new List<dynamic>();
                    foreach (var message in room.Value.messages)
                    {
                        origin.Add(message);
                    }

                    var sorted = origin.OrderBy(o => o.Value.timestamp).ToList();

                    if (sorted.Count <= RecordRemained) return;

                    for (int i = 0; i < sorted.Count - RecordRemained; i++)
                    {
                        var message = sorted[i];
                        TableChat c = _iDbService.TableChatData(room, message);
                        TableOperation insertOperation = TableOperation.Insert(c);
                        // Execute the insert operation.
                        var res = await table.ExecuteAsync(insertOperation);
                        if (res.HttpStatusCode == 204)
                        {
                            string url = "ChatRoom/" + room.Name + "/messages/" + message.Name;
                            var response = await _firebaseClient.DeleteAsync(url);
                            if (response.StatusCode != HttpStatusCode.OK)
                            {
                                Thread.Sleep(1000);
                                await _firebaseClient.DeleteAsync(url);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //wait and retry 5 times
                if (retry > 0)
                {
                    retry--;
                    Thread.Sleep(1000);
                    Backup(room, table, retry).Wait();
                }
                else
                {
                    Trace.TraceError("Error in Reading room message", e.Message);
                }
            }
        }
    }
}