using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LMS.model.Models;
using LMS.service.Service;
using Microsoft.Azure;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace StorageRole
{
    public class PostBackup
    {
        private static CloudTable _table;
        private static DocumentClient _client;
        private static Database _database;
        private static IDBService _iDbService;
        private static int reTry = 5;

        public PostBackup(CloudStorageAccount storageAccount, string endpointUrl, string authorizationKey)
        {
            _iDbService = new DBService();
            CloudTableClient c = storageAccount.CreateCloudTableClient();
            _table = c.GetTableReference("Post");
            _table.CreateIfNotExists();

            _client = new DocumentClient(new Uri(endpointUrl), authorizationKey);
            _database =
                _client.CreateDatabaseQuery().Where(db => db.Id == CloudConfigurationManager.GetSetting("Database"))
                    .AsEnumerable().FirstOrDefault();
        }

        public async Task BackupPostAll()
        {
            try
            {
                IEnumerable<DocumentCollection> dz = _client.CreateDocumentCollectionQuery(_database.SelfLink)
                    .AsEnumerable();
                foreach (var x in dz)
                {
                    await BackupPostCollection(x, 5);
                }
            }
            catch (DocumentClientException e)
            {
                Trace.TraceError("Error in updateDc " + e.Message);
                if (reTry > 0)
                {
                    reTry--;
                    Trace.TraceInformation("Restart BackupPost... in 1 min " + reTry);
                    Thread.Sleep(60000);
                    BackupPostAll().Wait();
                }
                else
                {
                    Trace.TraceError("Error in start" + e.Message);
                }
            }
        }

        private static async Task BackupPostCollection(DocumentCollection dc, int retryTimes)
        {
            Trace.TraceInformation("Collection '{0}' start.  Time: '{1}'", dc.Id,
                DateTime.Now.ToString(CultureInfo.CurrentCulture));
            try
            {
                var ds =
                    from d in _client.CreateDocumentQuery<PostMessage>(dc.DocumentsLink)
                    where d.Type == "Post"
                    select d;

                var docNumber = 0;
                TableBatchOperation batchOperation = new TableBatchOperation();
                List<String> documentList = new List<string>();
                foreach (var d in ds)
                {
                    TablePost c = _iDbService.TablePostData(d);
                    if (docNumber == 100)
                    {
                        await _table.ExecuteBatchAsync(batchOperation);
                        batchOperation = new TableBatchOperation();
                        docNumber = 0;
                        await _iDbService.DeleteDocByIdList(_client, dc, documentList,5);
                        documentList = new List<string>();
                    }
                    else
                    {
                        batchOperation.Insert(c);
                        documentList.Add(d.id);
                        docNumber++;
                    }
                }
                if (batchOperation.Count > 0)
                {
                    await _table.ExecuteBatchAsync(batchOperation);
                    await _iDbService.DeleteDocByIdList(_client, dc, documentList,5);
                }
            }
            catch (DocumentClientException e)
            {
                if (retryTimes > 0 && e.RetryAfter.TotalMilliseconds > 0)
                {
                    retryTimes--;
                    Thread.Sleep((int) e.RetryAfter.TotalMilliseconds);
                    BackupPostCollection(dc, retryTimes).Wait();
                }
                else
                {
                    Trace.TraceError("Error in BackupCollection " + e.Message);
                }
            }
            catch (StorageException e)
            {
                if (retryTimes > 0 &&
                    (e.RequestInformation.HttpStatusCode == 500 || e.RequestInformation.HttpStatusCode == 503))
                {
                    retryTimes--;
                    Thread.Sleep(1000);
                    BackupPostCollection(dc, retryTimes).Wait();
                }
                else
                {
                    Trace.TraceError("Error in BackupCollection " + e.Message);
                }
            }
        }
    }
}