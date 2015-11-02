using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LMS.Common.Models;
using LMS.Common.Service;
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
        private static IDbService _iDbService;
        private static int _reTry = 5;
        private static RetryPolicy<StorageTransientErrorDetectionStrategy> _retryPolicy;

        public PostBackup(CloudStorageAccount storageAccount, string endpointUrl, string authorizationKey)
        {
            _iDbService = new DbService(endpointUrl,authorizationKey);
            CloudTableClient c = storageAccount.CreateCloudTableClient();
            _table = c.GetTableReference("Post");
            _table.CreateIfNotExists();
            _retryPolicy = _iDbService.GetRetryPolicy();
        }

        public async Task BackupPostAll(string databaseSelfLink)
        {
            try
            {
                var client = _iDbService.GetDocumentClient();
                await client.OpenAsync();
                var collections = client.CreateDocumentCollectionQuery(databaseSelfLink)
                    .AsEnumerable();
                foreach (var dc in collections)
                {
                    await BackupPostCollection(dc,client);
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
                    BackupPostAll(databaseSelfLink).Wait();
                }
                else
                {
                    Trace.TraceError("Error in start" + e.Message);
                }
            }
        }

        private static async Task BackupPostCollection(DocumentCollection dc,DocumentClient client)
        {
            Trace.TraceInformation("Collection '{0}' start.  Time: '{1}'", dc.Id,
                DateTime.Now.ToString(CultureInfo.CurrentCulture));
            try
            {
                var ds =
                    from d in client.CreateDocumentQuery<PostMessage>(dc.DocumentsLink)
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


        public async Task CleanCollection(string databaseSelfLink, string masterCollectionSelfLink)
        {
            try
            {
                var client = _iDbService.GetDocumentClient();
                var collections = client.CreateDocumentCollectionQuery(databaseSelfLink)
                    .AsEnumerable();
                foreach (var dc in collections)
                {
                    if (dc.SelfLink == ConfigurationManager.AppSettings["MasterCollectionSelfLink"])
                    {
                        Offer offer = client.CreateOfferQuery()
                            .Where(r => r.ResourceLink == dc.SelfLink)
                            .AsEnumerable()
                            .SingleOrDefault();
                        if (offer != null)
                        {
                            offer.OfferType = "S3";
                            Offer updated = await client.ReplaceOfferAsync(offer);
                            await _iDbService.InitResolver(masterCollectionSelfLink);
                            await _iDbService.UpdateCurrentCollection(dc);
                        }
                    }
                    else
                    {
                        await client.DeleteDocumentCollectionAsync(dc.SelfLink);
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