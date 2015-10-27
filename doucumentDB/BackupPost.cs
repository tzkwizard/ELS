using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FireSharp.Interfaces;
using LMS.model.Models;
using LMS.service.Service;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace doucumentDB
{
    public class BackupPost
    {
        private static string storageConnectionString =
       "DefaultEndpointsProtocol=https;AccountName=elsaotuo;AccountKey=AV49N0PZ1Qlz42b0w47EPoPbNLULgxYOWxsO4IvFmrAkZPzkdGCKKOJqyiHVGfAPex6HhkDSWpNQAIuPmBHBMA==";
        private static IFirebaseClient _firebaseClient;
        private static string EndpointUrl = "https://tstaz.documents.azure.com:443/";

        private static string AuthorizationKey =
            "6xPkxpC7FyiozobQOtQ8yFxbqd7uLOCz0pRo4i+GKxHdmISxDrMKZdaKQH0/0BJe/xC3UKdQM4C1x5d4Rxk3AQ==";

        private const int RecordRemained = 2;

        private static CloudTable _table;
        private static DocumentClient _client;
        public static async Task BackupPostAll()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient c = storageAccount.CreateCloudTableClient();
            _table = c.GetTableReference("Post");
            _table.CreateIfNotExists();

            _client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
            var database = await DocumentDB.GetDB(_client);

            IEnumerable<DocumentCollection> dz = _client.CreateDocumentCollectionQuery(database.SelfLink)
               .AsEnumerable();
            foreach (var x in dz)
            {
               await BackupPostCollection(x);
            }
        }

        private static async Task DeletePost(DocumentCollection dc,string id)
        {
            var ds =
                 from d in _client.CreateDocumentQuery(dc.DocumentsLink)
                 where d.Id==id
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
                        await DeletePost(dc,d.id);                      
                        docNumber++;
                    }
                    else if (docNumber == 100)
                    {
                        await _table.ExecuteBatchAsync(batchOperation);
                        batchOperation = new TableBatchOperation();
                        docNumber = 0;
                    }
               
            }
            if (batchOperation.Count>0)
            {
                await _table.ExecuteBatchAsync(batchOperation);
            }
        }

    }
}
