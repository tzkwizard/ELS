using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.model.Models;
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
        public PostBackup(CloudStorageAccount storageAccount, string endpointUrl, string authorizationKey)
        {
            CloudTableClient c = storageAccount.CreateCloudTableClient();
            _table = c.GetTableReference("Post");
            _table.CreateIfNotExists();

            _client = new DocumentClient(new Uri(endpointUrl), authorizationKey);

            _database = _client.CreateDatabaseQuery().Where(db => db.Id == CloudConfigurationManager.GetSetting("Database"))
                .AsEnumerable().FirstOrDefault();
        }

        public async Task BackupPostAll()
        {
            IEnumerable<DocumentCollection> dz = _client.CreateDocumentCollectionQuery(_database.SelfLink)
               .AsEnumerable();
            foreach (var x in dz)
            {
                await BackupPostCollection(x);
            }
        }
        private static async Task DeletePost(DocumentCollection dc, string id)
        {
            var ds =
                 from d in _client.CreateDocumentQuery(dc.DocumentsLink)
                 where d.Id == id
                 select d;
            foreach (var d in ds)
            {
                await _client.DeleteDocumentAsync(d.SelfLink);
            }
        }

        private static async Task BackupPostCollection(DocumentCollection dc)
        {

            var ds =
                  from d in _client.CreateDocumentQuery<PostMessage>(dc.DocumentsLink)
                  where d.Type == "Post"
                  select d;
            
            var docNumber = 0;
            TableBatchOperation batchOperation = new TableBatchOperation();
            foreach (var d in ds)
            {
                TablePost c = new TablePost(d.Type, d.id)
                {
                    district = d.Path.District,
                    school = d.Path.School,
                    classes = d.Path.Classes,
                    timestamp = d.Info.timestamp,
                    user = d.Info.user,
                    uid = d.Info.uid,
                    message = d.Info.message
                };

                if (docNumber < 100)
                {
                    batchOperation.Insert(c);
                    await DeletePost(dc, d.id);
                    docNumber++;
                }
                else if (docNumber == 100)
                {
                    await _table.ExecuteBatchAsync(batchOperation);
                    batchOperation = new TableBatchOperation();
                    docNumber = 0;
                }

            }
            if (batchOperation.Count > 0)
            {
                await _table.ExecuteBatchAsync(batchOperation);
            }
        }


    }
}
