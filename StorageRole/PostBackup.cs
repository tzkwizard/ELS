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
        private static int _reTry = 5;
        private static RetryPolicy<StorageTransientErrorDetectionStrategy> _retryPolicy;

        public PostBackup(CloudStorageAccount storageAccount, string endpointUrl, string authorizationKey)
        {
            _iDbService = new DBService(endpointUrl,authorizationKey);
            CloudTableClient c = storageAccount.CreateCloudTableClient();
            _table = c.GetTableReference("Post");
            _table.CreateIfNotExists();

            _client = _iDbService.GetDocumentClient();
            _database =
                _client.CreateDatabaseQuery().Where(db => db.Id == CloudConfigurationManager.GetSetting("Database"))
                    .AsEnumerable().FirstOrDefault();
            _retryPolicy = _iDbService.GetRetryPolicy();
        }

        public async Task BackupPostAll()
        {
            try
            {
                await _iDbService.OpenDB();
                var collections = _client.CreateDocumentCollectionQuery(_database.SelfLink)
                    .AsEnumerable();
                foreach (var dc in collections)
                {
                    await BackupPostCollection(dc);
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error in updateDc " + e.Message);
                if (_reTry > 0)
                {
                    _reTry--;
                    Trace.TraceInformation("Restart BackupPost... in 1 min " + _reTry);
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
                List<dynamic> docList = new List<dynamic>();
                foreach (var d in ds)
                {
                    TablePost c = _iDbService.TablePostData(d);
                    batchOperation.Insert(c);
                    docList.Add(d);

                    if (batchOperation.Count == 100)
                    {
                        var operation = batchOperation;
                        var res = await _retryPolicy.ExecuteAsync(
                            () => _table.ExecuteBatchAsync(operation));
                        batchOperation = new TableBatchOperation();
                        if (res.Count == operation.Count)
                        {
                            await _iDbService.BatchDelete(dc,docList);
                            docList = new List<dynamic>();
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
                        await _iDbService.BatchDelete(dc,docList);
                        Trace.TraceInformation("inserted");
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error in BackupCollection " + e.Message);
            }
        }


        public async Task CleanCollection()
        {
            try
            {
                var collections = _client.CreateDocumentCollectionQuery(_database.SelfLink)
                    .AsEnumerable();
                foreach (var dc in collections)
                {
                    if (dc.Id == CloudConfigurationManager.GetSetting("MasterCollection"))
                    {
                        Offer offer = _client.CreateOfferQuery()
                            .Where(r => r.ResourceLink == dc.SelfLink)
                            .AsEnumerable()
                            .SingleOrDefault();
                        if (offer != null)
                        {
                            offer.OfferType = "S3";
                            Offer updated = await _client.ReplaceOfferAsync(offer);
                            await _iDbService.InitResolver();
                            await _iDbService.UpdateCurrentCollection(dc);
                        }
                    }
                    else
                    {
                        await _client.DeleteDocumentCollectionAsync(dc.SelfLink);
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error in Clean Collection" + e.Message);
            }
        }
    }
}