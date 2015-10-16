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
using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.AzureStorage;
using Microsoft.Practices.TransientFaultHandling;
using Microsoft.ServiceBus;
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
        private static RetryPolicy<StorageTransientErrorDetectionStrategy> _retryPolicy;

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
            _retryPolicy = _iDbService.GetRetryPolicy();
        }

        public async Task BackupPostAll()
        {
            try
            {
                IEnumerable<DocumentCollection> dz = _client.CreateDocumentCollectionQuery(_database.SelfLink)
                    .AsEnumerable();
                foreach (var x in dz)
                {
                    await BackupPostCollection(x);
                }
            }
            catch (Exception e)
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

        private static async Task BackupPostCollection(DocumentCollection dc)
        {
            Trace.TraceInformation("Collection '{0}' start.  Time: '{1}'", dc.Id,
                DateTime.Now.ToString(CultureInfo.CurrentCulture));
            try
            {
                var ds =
                    from d in _client.CreateDocumentQuery<PostMessage>(dc.DocumentsLink)
                    where d.Type == "Post"
                    select d;

                TableBatchOperation batchOperation = new TableBatchOperation();
                List<String> documentList = new List<string>();
                foreach (var d in ds)
                {
                    TablePost c = _iDbService.TablePostData(d);
                    batchOperation.Insert(c);
                    documentList.Add(d.id);

                    if (batchOperation.Count == 100)
                    {
                        var operation = batchOperation;
                        var res = await _retryPolicy.ExecuteAsync(
                            () => _table.ExecuteBatchAsync(operation));
                        batchOperation = new TableBatchOperation();
                        if (res.Count == operation.Count)
                        {
                            await _iDbService.DeleteDocByIdList(_client, dc, documentList, 5);
                            documentList = new List<string>();
                            Trace.TraceInformation("inserted");
                        }
                    }
                }
                if (batchOperation.Count > 0)
                {
                    var operation = batchOperation;
                    var res = await _retryPolicy.ExecuteAsync(
                        () => _table.ExecuteBatchAsync(operation));
                    if (res.Count == operation.Count)
                    {
                        await _iDbService.DeleteDocByIdList(_client, dc, documentList, 5);
                        Trace.TraceInformation("inserted");
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error in BackupCollection " + e.Message);
            }
        }
    }
}