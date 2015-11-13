using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LMS.Common.Models;
using LMS.Common.Service;
using Microsoft.Azure;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents.Partitioning;

namespace LMS.Common.DAL
{
    public class DBoperation : IDBoperation
    {
        private static DocumentClient _documentClient;
        private static string _endpointUrl;
        private static string _authorizationKey;
        private static string _dataSelfLink;
        private static string _masterCollectionSelfLink;
        private static StoredProcedure _batchDelete;

        public DBoperation()
        {
            _masterCollectionSelfLink = ConfigurationManager.AppSettings["MasterCollectionSelfLink"] ??
                                        CloudConfigurationManager.GetSetting("MasterCollectionSelfLink");
            _dataSelfLink = ConfigurationManager.AppSettings["DBSelfLink"] ??
                            CloudConfigurationManager.GetSetting("DBSelfLink");
            _endpointUrl = ConfigurationManager.AppSettings["DocumentDBUrl"] ??
                           CloudConfigurationManager.GetSetting("DocumentDBUrl");
            _authorizationKey = ConfigurationManager.AppSettings["DocumentDBAuthorizationKey"] ??
                                CloudConfigurationManager.GetSetting("DocumentDBAuthorizationKey");
        }

        #region Database,Collection,client

        public Database GetDd(string dName)
        {
            var client = GetDocumentClient();
            var database = client.CreateDatabaseQuery().Where(db => db.Id == dName)
                .AsEnumerable().FirstOrDefault();

            return database;
        }

        public DocumentCollection GetDc(string cName, string dName)
        {
            var client = GetDocumentClient();
            var database = GetDd(dName);
            DocumentCollection documentCollection = client.CreateDocumentCollectionQuery(database.SelfLink)
                .Where(c => c.Id == cName)
                .AsEnumerable()
                .FirstOrDefault();

            return documentCollection;
        }

        public DocumentCollection GetCurrentDc()
        {
            var client = GetDocumentClient();
            var curDoc =
                client.CreateDocumentQuery<CurrentCollection>(_masterCollectionSelfLink)
                    .Where(x => x.id == "CurrentCollection")
                    .AsEnumerable()
                    .FirstOrDefault();
            if (curDoc != null)
            {
                return client.CreateDocumentCollectionQuery(_dataSelfLink)
                    .Where(c => c.Id == curDoc.name)
                    .AsEnumerable()
                    .FirstOrDefault();
            }

            return null;
        }

        public DocumentClient GetDocumentClient()
        {
            return _documentClient ?? (_documentClient = new DocumentClient(new Uri(_endpointUrl), _authorizationKey));
        }

        public void UpdateDbClientResolver(RangePartitionResolver<long> rangeResolver)
        {
            _documentClient = GetDocumentClient();
            _documentClient.PartitionResolvers[_dataSelfLink] = rangeResolver;
        }

        #endregion

        #region StoreProcedure

        private static StoredProcedure BatchDelete()
        {
            var sp = new StoredProcedure
            {
                Id = "BatchDelete",
                Body = @"
function batchDelete(docs) {
    var collection = getContext().getCollection();
    var collectionLink = collection.getSelfLink();

    // The count of imported docs, also used as current doc index.
    var count = 0;

	var docsLength = docs.length;

    var res=[];
	var unres=[]
	tryCreate(docs[count], callback);
    
    function tryCreate(doc, callback) {
		   var link=doc._self;
           var isAccepted = collection.deleteDocument(link, callback);
           if (!isAccepted) getContext().getResponse().setBody(res);
    }

    // This is called when collection.createDocument is done in order to process the result.
    function callback(err, doc, options) {
        if (err) throw err;
        res.push(docs[count]);
        // One more document has been inserted, increment the count.
        count++;
        
        if (count >= docsLength) {
            // If we created all documents, we are done. Just set the response.
            getContext().getResponse().setBody(res);
        } else {
            // Create next document.
            tryCreate(docs[count], callback);
        }
    }
}              "
            };
            return sp;
        }

        private static StoredProcedure BatchInsert()
        {
            var sp = new StoredProcedure
            {
                Id = "BatchInsert",
                Body = @"
function bulkImport(docs) {
    var collection = getContext().getCollection();
    var collectionLink = collection.getSelfLink();

    // The count of imported docs, also used as current doc index.
    var count = 0;

    // Validate input.
    if (!docs) throw new Error('The array is undefined or null.');
    var res=[];
    var docsLength = docs.length;
    if (docsLength == 0) {
        getContext().getResponse().setBody(res);
    }
	
    // Call the create API to create a document.
    tryCreate(docs[count], callback);

    // Note that there are 2 exit conditions:
    // 1) The createDocument request was not accepted. 
    //    In this case the callback will not be called, we just call setBody and we are done.
    // 2) The callback was called docs.length times.
    //    In this case all documents were created and we don’t need to call tryCreate anymore. Just call setBody and we are done.
    function tryCreate(doc, callback) {
        var isAccepted = collection.createDocument(collectionLink, doc, callback);

        // If the request was accepted, callback will be called.
        // Otherwise report current count back to the client, 
        // which will call the script again with remaining set of docs.
        if (!isAccepted) getContext().getResponse().setBody(res);
    }

    // This is called when collection.createDocument is done in order to process the result.
    function callback(err, doc, options) {
        if (err) throw err;
        res.push(docs[count]);
        // One more document has been inserted, increment the count.
        count++;

        if (count >= docsLength) {
            // If we created all documents, we are done. Just set the response.
            getContext().getResponse().setBody(res);
        } else {
            // Create next document.
            tryCreate(docs[count], callback);
        }
    }
} "
            };
            return sp;
        }

        public async Task<StoredProcedure> GetStoreProcedure(string dcLink, string spName)
        {
            var client = GetDocumentClient();
            var sp = client.CreateStoredProcedureQuery(dcLink).Where(c => c.Id == spName)
                .AsEnumerable()
                .FirstOrDefault();
            if (sp != null) return sp;
            if (spName == "BatchDelete")
            {
                sp = await client.CreateStoredProcedureAsync(dcLink, BatchDelete());
            }
            if (spName == "BatchInsert")
            {
                sp = await client.CreateStoredProcedureAsync(dcLink, BatchInsert());
            }

            return sp;
        }

        public async Task BatchDelete(DocumentCollection dc, List<dynamic> docs)
        {
            var client = GetDocumentClient();
            var sp = _batchDelete ??
                     (client.CreateStoredProcedureQuery(dc.SelfLink).Where(c => c.Id == "BatchDelete")
                         .AsEnumerable()
                         .FirstOrDefault() ?? await client.CreateStoredProcedureAsync(dc.SelfLink, BatchDelete()));
            _batchDelete = sp;
            var res = await RetryService.ExecuteWithRetries(() => client.ExecuteStoredProcedureAsync<List<Document>>(
                sp.SelfLink, docs));


            //Process if some delete fail in batchDelete 
            if (res.Response.Count != docs.Count)
            {
                var cur = res.Response.Count;
                while (cur < docs.Count)
                {
                    var s = new List<dynamic>();
                    for (var i = cur; i < docs.Count; i++)
                    {
                        if (s.Count < docs.Count)
                        {
                            s.Add(docs[i]);
                        }
                    }
                    var res2 =
                        await RetryService.ExecuteWithRetries(() => client.ExecuteStoredProcedureAsync<List<Document>>(
                            sp.SelfLink, s));
                    cur = cur + res2.Response.Count;
                }
            }
        }

        #endregion

        #region Document 

        public async Task DeleteDocByIdList(DocumentCollection dc, List<string> idList)
        {
            foreach (var id in idList)
            {
                await DeleteDocById(dc.SelfLink, id);
            }
        }

        public async Task DeleteDocById(string selfLink, string id)
        {
            var link = selfLink ?? _masterCollectionSelfLink;
            var client = GetDocumentClient();
            var doc =
                (from d in client.CreateDocumentQuery(link)
                    where d.Id == id
                    select d).AsEnumerable().FirstOrDefault();
            if (doc != null)
            {
                await RetryService.ExecuteWithRetries(() => client.DeleteDocumentAsync(doc.SelfLink));
            }
        }

        public async Task<bool> CreateDocument(string dclink, object doc)
        {
            var link = dclink ?? _masterCollectionSelfLink;
            var client = GetDocumentClient();
            var res = await
                RetryService.ExecuteWithRetries(
                    () => client.CreateDocumentAsync(link, doc));
            return res.StatusCode == HttpStatusCode.Created;
        }

        public async Task<bool> CreateDocument(object doc)
        {
            var client = GetDocumentClient();
            var res = await
                RetryService.ExecuteWithRetries(
                    () => client.CreateDocumentAsync(_masterCollectionSelfLink, doc));
            return res.StatusCode == HttpStatusCode.Created;
        }

        public async Task ReplaceDocument(dynamic item)
        {
            var client = GetDocumentClient();
            await RetryService.ExecuteWithRetries(() => client.ReplaceDocumentAsync(item));
        }

        public Document GetDocumentById(string dcLink, string id)
        {
            var link = dcLink ?? _masterCollectionSelfLink;
            var client = GetDocumentClient();
            return client.CreateDocumentQuery(link)
                .Where(x => x.Id == id)
                .AsEnumerable()
                .FirstOrDefault();
        }

        public dynamic GetDocumentByType(string dcLink, string type)
        {
            var client = GetDocumentClient();
            return client.CreateDocumentQuery<PostMessage>(dcLink)
                .Where(x => x.Type == type)
                .AsEnumerable()
                .FirstOrDefault();
        }

        #endregion

        #region Query

        public List<PostMessage> GetPostMessages(long start, long end)
        {
            var client = GetDocumentClient();
            var dataSelfLink = CloudConfigurationManager.GetSetting("DBSelfLink") ??
                               ConfigurationManager.AppSettings["DBSelfLink"];
            /* return client.CreateDocumentQuery<T>(dataSelfLink,
                    "SELECT d AS data " +
                    "FROM Doc d " +
                    "Where d.Type='Post' And d.Info.timestamp > '" + start + "'" +
                    "And d.Info.timestamp < '" + end + "'").ToList();*/

            return client.CreateDocumentQuery<PostMessage>(dataSelfLink)
                .Where(f => f.Type == "Post" && f.Info.timestamp > start && f.Info.timestamp < end)
                .OrderBy(o => o.Info.timestamp).ToList();
        }

        public List<PostMessage> GetPostMessages(long end)
        {
            var client = GetDocumentClient();
            /*   items = client.CreateDocumentQuery<PostMessage>(documentCollection.DocumentsLink,
                    "SELECT d AS data " +
                    "FROM Doc d " +
                    "Where d.Type='Post' And d.Info.timestamp > '" + time + "'").OrderBy(o=>o.Info.timestamp).ToList();*/

            var dataSelfLink = CloudConfigurationManager.GetSetting("DBSelfLink") ??
                               ConfigurationManager.AppSettings["DBSelfLink"];

            return client.CreateDocumentQuery<PostMessage>(dataSelfLink,
                new FeedOptions {MaxItemCount = 2000})
                .Where(f => f.Type == "Post" && f.Info.timestamp > end)
                .OrderBy(o => o.Info.timestamp).ToList();
        }

        #endregion
    }
}