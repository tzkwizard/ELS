using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
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

        public ChatBackup(CloudStorageAccount storageAccount)
        {
            CloudTableClient c = storageAccount.CreateCloudTableClient();
            _table = c.GetTableReference("Chat");
            _table.CreateIfNotExists();
            IDBService i = new DBService();
            _firebaseClient = i.GetFirebaseClient(CloudConfigurationManager.GetSetting("Firebasenode"));
        }

        public async Task BackupDocumentChat()
        {
            try
            {
                FirebaseResponse chatResponse = _firebaseClient.Get("ChatRoom");
                dynamic ds = JsonConvert.DeserializeObject(chatResponse.Body);

                foreach (var d in ds)
                {
                    dynamic t = d;
                    await Backup(t, _table, 5);
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error in BackupChat " + e.Message);
                if (reTry > 0)
                {
                    Trace.TraceInformation("Retry...  " + reTry);
                    BackupDocumentChat().Wait();
                }
                else
                {
                    Trace.TraceError("Run out of retry");
                }
                reTry--;
            }
        }

        private async Task Backup(dynamic u, CloudTable table, int retry)
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
                        TableChat c = new TableChat(u.Value.room.ID.ToString(), s.Name.ToString())
                        {
                            roomName = u.Value.room.Name,
                            timestamp = s.Value.timestamp,
                            user = s.Value.user,
                            uid = s.Value.uid,
                            message = s.Value.message
                        };
                        TableOperation insertOperation = TableOperation.Insert(c);
                        // Execute the insert operation.
                        var res = await table.ExecuteAsync(insertOperation);
                        if (res.HttpStatusCode == 204)
                        {
                            string url = "ChatRoom/" + u.Name + "/messages/" + s.Name;
                            await _firebaseClient.DeleteAsync(url);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                while (retry > 0)
                {
                    Backup(u, table, retry--);
                }
                Trace.TraceError("Error in Room " + u + e.Message);
            }
        }
    }
}