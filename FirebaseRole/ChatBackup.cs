using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using FireSharp.Exceptions;
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
            catch (FirebaseException e)
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
                    var docNumber = 0;
                    TableBatchOperation batchOperation = new TableBatchOperation();

                    for (int i = 0; i < sorted.Count - RecordRemained; i++)
                    {
                        var message = sorted[i];
                        TableChat c = _iDbService.TableChatData(room, message);

                        if (docNumber == 100)
                        {
                            var res = await _table.ExecuteBatchAsync(batchOperation);
                            await DeleteFirebase(res);
                            batchOperation = new TableBatchOperation();
                            docNumber = 0;
                        }
                        else
                        {
                            batchOperation.Insert(c);
                            docNumber++;
                        }
                    }

                    if (batchOperation.Count > 0)
                    {
                        var res = await _table.ExecuteBatchAsync(batchOperation);
                        await DeleteFirebase(res);
                    }
                }
            }
            catch (StorageException e)
            {
                //retry is retryable
                if (retry > 0 &&
                    (e.RequestInformation.HttpStatusCode == 500 || e.RequestInformation.HttpStatusCode == 503))
                {
                    retry--;
                    Thread.Sleep(1000);
                    Backup(room, table, retry).Wait();
                }
                else
                {
                    Trace.TraceError("Error in room : " + room.Name + e.Message);
                }
            }
        }

        private async Task DeleteFirebase(IList<TableResult> res)
        {
            foreach (var item in res)
            {
                try
                {
                    TableChat i = (TableChat) item.Result;
                    string url = "ChatRoom/" + i.PartitionKey + "/messages/" + i.RowKey;
                    await _firebaseClient.DeleteAsync(url);
                }
                catch (FirebaseException e)
                {
                    Trace.TraceError("Error in Firebase : " + e.Message);
                }
            }
        }
    }
}