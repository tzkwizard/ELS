﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FireSharp;
using FireSharp.Interfaces;
using FireSharp.Config;
using LMS.model.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents.Partitioning;
using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.AzureStorage;
using Microsoft.Practices.TransientFaultHandling;

namespace LMS.service.Service
{
    public class DBService : IDBService
    {
        private static string EndpointUrl = "https://tstaz.documents.azure.com:443/";

        private static string AuthorizationKey =
            "6xPkxpC7FyiozobQOtQ8yFxbqd7uLOCz0pRo4i+GKxHdmISxDrMKZdaKQH0/0BJe/xC3UKdQM4C1x5d4Rxk3AQ==";

        private readonly DocumentClient _documentClient;
        private const int MaxMonthTime = 1;
        private string firebaseSecret = "F1EIaYtnYgfkVVI7sSBe3WDyUMlz4xV6jOrxIuxO";
        private const int RetryTimes = 5;

        public DBService(string endpointUrl, string authorizationKey)
        {
            _documentClient = new DocumentClient(new Uri(endpointUrl), authorizationKey);
        }

        public DBService()
        {
            _documentClient = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
        }

        public List<Topic> GetCalendar()
        {
            var documentCollection = GetDc(_documentClient, "LMSCollection", "LMSRegistry");
            var item =
                from f in _documentClient.CreateDocumentQuery<Topic>(documentCollection.DocumentsLink)
                where f.Type == "Topic"
                select f;
            return item.ToList();
        }

        public string GetFirebaseToken(string user, string uid, string data)
        {
            var tokenGenerator = new Firebase.TokenGenerator(firebaseSecret);
            var authPayload = new Dictionary<string, object>()
            {
                {"uid", uid},
                {"user", user},
                {"data", data}
            };
            string token = tokenGenerator.CreateToken(authPayload);
            return token;
        }

        public LMSresult GetList(string m)
        {
            long end = 0;
            var n = 0;
            List<PostMessage> items = new List<PostMessage>();
            while (items.Count < 5 && n < 6)
            {
                end =
                    (long) (DateTime.UtcNow.AddMinutes(-30*n).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
                var documentCollection = GetDc(_documentClient, "LMSCollection", "LMSRegistry");
                var path = m.Split('/');
                /*   items = _documentClient.CreateDocumentQuery<PostMessage>(documentCollection.DocumentsLink,
                    "SELECT d AS data " +
                    "FROM Doc d " +
                    "Where d.Type='Post' And d.Info.timestamp > '" + time + "'").OrderBy(o=>o.Info.timestamp).ToList();*/

                items =
                    (from f in _documentClient.CreateDocumentQuery<PostMessage>(documentCollection.DocumentsLink)
                        where f.Type == "Post" && f.Info.timestamp > end
                        select f).OrderBy(o => o.Info.timestamp).ToList();
                n++;
            }

            var res = new LMSresult
            {
                time = end,
                list = items
            };
            return res;
        }

        public LMSresult GetMoreList(string m, long start)
        {
            var t = (long) (DateTime.UtcNow.AddMonths(-1).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            List<PostMessage> items = new List<PostMessage>();
            var t1 = DateTime.Now;
            var n = 1;
            var i = 1;
            var end = start;
            while (items.Count < 5 && end > t)
            {
                i = i + n;
                end = end - (long) TimeSpan.FromHours(i).TotalMilliseconds;
                var documentCollection = GetDc(_documentClient, "LMSCollection", "LMSRegistry");
                var path = m.Split('/');
                /*  items = _documentClient.CreateDocumentQuery<PostMessage>(documentCollection.DocumentsLink,
                    "SELECT d AS data " +
                    "FROM Doc d " +
                    "Where d.Type='Post' And d.Info.timestamp > '" + t2 + "'" +
                    "And d.Info.timestamp < '" + t1 + "'").ToList();*/
                items =
                    (from f in _documentClient.CreateDocumentQuery<PostMessage>(documentCollection.DocumentsLink)
                        where f.Type == "Post" && f.Info.timestamp < start && f.Info.timestamp > end
                        select f).OrderBy(o => o.Info.timestamp).ToList();
                var t2 = DateTime.Now;
                if (t2 - t1 > TimeSpan.FromSeconds(10))
                {
                    return new LMSresult
                    {
                        moreData = true,
                        time = end,
                        list = items
                    };
                }
                n++;
            }
            var res = new LMSresult
            {
                moreData = false,
                time = end,
                list = items
            };
            return res;
        }


        public Database GetDd(DocumentClient client, string dName)
        {
            Database database = client.CreateDatabaseQuery().Where(db => db.Id == dName)
                .AsEnumerable().FirstOrDefault();

            return database;
        }

        public DocumentCollection GetDc(DocumentClient client, string cName, string dName)
        {
            var database = GetDd(client, dName);
            DocumentCollection documentCollection = client.CreateDocumentCollectionQuery(database.SelfLink)
                .Where(c => c.Id == cName)
                .AsEnumerable()
                .FirstOrDefault();

            return documentCollection;
        }

        public StoredProcedure GetSp(DocumentClient client, DocumentCollection documentCollection, string spName)
        {
            var sp = client.CreateStoredProcedureQuery(documentCollection.SelfLink).Where(c => c.Id == spName)
                .AsEnumerable()
                .FirstOrDefault();
            return sp;
        }

        public IFirebaseClient GetFirebaseClient()
        {
            var node = "https://dazzling-inferno-4653.firebaseio.com/";
            var firebaseSecret = "F1EIaYtnYgfkVVI7sSBe3WDyUMlz4xV6jOrxIuxO";
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = firebaseSecret,
                BasePath = node
            };
            IFirebaseClient client = new FirebaseClient(config);
            return client;
        }

        public IFirebaseClient GetFirebaseClient(string node)
        {
            var firebaseSecret = "F1EIaYtnYgfkVVI7sSBe3WDyUMlz4xV6jOrxIuxO";
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = firebaseSecret,
                BasePath = node
            };
            IFirebaseClient client = new FirebaseClient(config);
            return client;
        }


        public DocumentClient GetDocumentClient()
        {
            var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
            return client;
        }


        public DocumentCollection SearchCollection(string dis, DocumentCollection masterCollection, Database database)
        {
            var ds =
                from d in _documentClient.CreateDocumentQuery<DcAllocate>(masterCollection.DocumentsLink)
                where d.Type == "DisList"
                select d;
            foreach (var d in ds)
            {
                if (d.District.Contains(dis))
                {
                    DocumentCollection dc = _documentClient.CreateDocumentCollectionQuery(database.SelfLink)
                        .Where(c => c.Id == d.DcName)
                        .AsEnumerable()
                        .FirstOrDefault();
                    if (dc != null)
                    {
                        return dc;
                    }
                }
            }
            return masterCollection;
        }


        public async Task DeleteDocByIdList(DocumentClient client, DocumentCollection dc, List<string> idList,
            int retryTimes)
        {
            foreach (var id in idList)
            {
                var id1 = id;
                var ds =
                    from d in client.CreateDocumentQuery(dc.DocumentsLink)
                    where d.Id == id1
                    select d;
                foreach (var d in ds)
                {
                    //await client.DeleteDocumentAsync(d.SelfLink);
                    //await DeleteDocument(client, d.SelfLink, retryTimes);
                    var d1 = d;
                    await ExecuteWithRetries(retryTimes, () => client.DeleteDocumentAsync(d1.SelfLink));
                }
            }
        }

        public async Task DeleteDocById(DocumentClient client, DocumentCollection dc, string id, int retryTimes)
        {
            var ds =
                from d in client.CreateDocumentQuery(dc.DocumentsLink)
                where d.Id == id
                select d;
            foreach (var d in ds)
            {
                var d1 = d;
                await ExecuteWithRetries(retryTimes, () => client.DeleteDocumentAsync(d1.SelfLink));
            }
        }

        public PostMessage PostData(dynamic data, string[] path)
        {
            Random r = new Random();
            PostMessage message = new PostMessage
            {
                Type = "Post",
                Path = new PostPath()
                {
                    District = path[1] + r.Next(1, 51),
                    School = path[2],
                    Classes = path[3]
                },
                Info = new Info()
                {
                    user = data.body.user,
                    uid = data.body.uid,
                    timestamp = data.body.timestamp,
                    message = data.body.message
                }
            };
            return message;
        }

        public TableChat TableChatData(dynamic room, dynamic message)
        {
            TableChat item = new TableChat(room.Value.room.ID.ToString(), message.Name.ToString())
            {
                roomName = room.Value.room.Name,
                timestamp = message.Value.timestamp,
                user = message.Value.user,
                uid = message.Value.uid,
                message = message.Value.message
            };
            return item;
        }


        public TablePost TablePostData(dynamic post)
        {
            TablePost item = new TablePost(post.Type, post.id)
            {
                district = post.Path.District,
                school = post.Path.School,
                classes = post.Path.Classes,
                timestamp = post.Info.timestamp,
                user = post.Info.user,
                uid = post.Info.uid,
                message = post.Info.message
            };
            return item;
        }


        public async Task DeleteDocument(DocumentClient client, string selfLink, int retryTimes)
        {
            try
            {
                await client.DeleteDocumentAsync(selfLink);
            }
            catch (DocumentClientException e)
            {
                if (e.RetryAfter.TotalMilliseconds > 0)
                {
                    retryTimes--;
                    Thread.Sleep((int) e.RetryAfter.TotalMilliseconds);
                    DeleteDocument(client, selfLink, retryTimes).Wait();
                }
                else
                {
                    Trace.TraceError("Error in Delete Document" + e.Message);
                }
            }
        }

        public async Task ReplaceDocument(DocumentClient client, dynamic item, int retryTimes)
        {
            try
            {
                await client.ReplaceDocumentAsync(item);
            }
            catch (DocumentClientException e)
            {
                if (e.RetryAfter.TotalMilliseconds > 0)
                {
                    retryTimes--;
                    Thread.Sleep((int) e.RetryAfter.TotalMilliseconds);
                    ReplaceDocument(client, item, retryTimes).Wait();
                }
                else
                {
                    Trace.TraceError("Error in Replace Document" + e.Message);
                }
            }
        }

        public async Task<ResourceResponse<Document>> AddDocument(DocumentClient client, string dcSelfLink, dynamic item,
            int retryTimes)
        {
            try
            {
                return await client.CreateDocumentAsync(dcSelfLink, item);
            }
            catch (DocumentClientException e)
            {
                if (e.RetryAfter.TotalMilliseconds > 0)
                {
                    retryTimes--;
                    Thread.Sleep((int) e.RetryAfter.TotalMilliseconds);
                    AddDocument(client, dcSelfLink, item, retryTimes).Wait();
                }
                else
                {
                    Trace.TraceError("Error in Add Document" + e.Message);
                }
                return null;
            }
        }


        public async Task<ResourceResponse<Document>> ExecuteWithRetries(
            Func<Task<ResourceResponse<Document>>> function)
        {
            TimeSpan sleepTime = TimeSpan.FromMilliseconds(100);
            var retryTimes = RetryTimes;
            while (retryTimes > 0 && sleepTime != TimeSpan.Zero)
            {
                try
                {
                    return await function();
                }
                catch (DocumentClientException e)
                {
                    if (e.StatusCode != null && (int) e.StatusCode != 429)
                    {
                        Trace.TraceInformation(e.Message);
                    }
                    sleepTime = e.RetryAfter;
                }
                catch (AggregateException ae)
                {
                    if (!(ae.InnerException is DocumentClientException))
                    {
                        Trace.TraceInformation(ae.Message);
                    }

                    DocumentClientException e = (DocumentClientException) ae.InnerException;
                    if (e.StatusCode != null && (int) e.StatusCode != 429)
                    {
                        Trace.TraceInformation(e.Message);
                    }
                    sleepTime = e.RetryAfter;
                }
                retryTimes--;
                await Task.Delay(sleepTime);
            }
            return null;
        }

        public async Task ExecuteWithRetries(
            Func<Task> function)
        {
            TimeSpan sleepTime = TimeSpan.FromMilliseconds(100);
            var retryTimes = RetryTimes;
            while (retryTimes > 0 && sleepTime != TimeSpan.Zero)
            {
                try
                {
                    await function();
                    break;
                }
                catch (DocumentClientException e)
                {
                    if (e.StatusCode != null && (int) e.StatusCode != 429)
                    {
                        Trace.TraceInformation(e.Message);
                    }
                    sleepTime = e.RetryAfter;
                }
                catch (AggregateException ae)
                {
                    if (!(ae.InnerException is DocumentClientException))
                    {
                        Trace.TraceInformation(ae.Message);
                    }

                    DocumentClientException e = (DocumentClientException) ae.InnerException;
                    if (e.StatusCode != null && (int) e.StatusCode != 429)
                    {
                        Trace.TraceInformation(e.Message);
                    }
                    sleepTime = e.RetryAfter;
                }
                retryTimes--;
                await Task.Delay(sleepTime);
            }
        }

        public async Task<ResourceResponse<Document>> ExecuteWithRetries(int retryTimes,
            Func<Task<ResourceResponse<Document>>> function)
        {
            TimeSpan sleepTime = TimeSpan.FromMilliseconds(100);

            while (retryTimes > 0 && sleepTime != TimeSpan.Zero)
            {
                try
                {
                    return await function();
                }
                catch (DocumentClientException e)
                {
                    if (e.StatusCode != null && (int) e.StatusCode != 429)
                    {
                        Trace.TraceInformation(e.Message);
                    }
                    sleepTime = e.RetryAfter;
                }
                catch (AggregateException ae)
                {
                    if (!(ae.InnerException is DocumentClientException))
                    {
                        Trace.TraceInformation(ae.Message);
                    }

                    DocumentClientException e = (DocumentClientException) ae.InnerException;
                    if (e.StatusCode != null && (int) e.StatusCode != 429)
                    {
                        Trace.TraceInformation(e.Message);
                    }
                    sleepTime = e.RetryAfter;
                }
                retryTimes--;
                await Task.Delay(sleepTime);
            }
            return null;
        }

        public async Task ExecuteWithRetries(int retryTimes,
            Func<Task> function)
        {
            TimeSpan sleepTime = TimeSpan.FromMilliseconds(100);

            while (retryTimes > 0 && sleepTime != TimeSpan.Zero)
            {
                try
                {
                    await function();
                    break;
                }
                catch (DocumentClientException e)
                {
                    if (e.StatusCode != null && (int) e.StatusCode != 429)
                    {
                        Trace.TraceInformation(e.Message);
                    }
                    sleepTime = e.RetryAfter;
                }
                catch (AggregateException ae)
                {
                    if (!(ae.InnerException is DocumentClientException))
                    {
                        Trace.TraceInformation(ae.Message);
                    }

                    DocumentClientException e = (DocumentClientException) ae.InnerException;
                    if (e.StatusCode != null && (int) e.StatusCode != 429)
                    {
                        Trace.TraceInformation(e.Message);
                    }
                    sleepTime = e.RetryAfter;
                }
                retryTimes--;
                await Task.Delay(sleepTime);
            }
        }

        public async Task<StoredProcedureResponse<List<Document>>> ExecuteWithRetries(
            Func<Task<StoredProcedureResponse<List<Document>>>> function)
        {
            TimeSpan sleepTime = TimeSpan.FromMilliseconds(100);
            var retryTimes = RetryTimes;
            while (retryTimes > 0 && sleepTime != TimeSpan.Zero)
            {
                try
                {
                    return await function();
                }
                catch (DocumentClientException e)
                {
                    if (e.StatusCode != null && (int) e.StatusCode != 429)
                    {
                        Trace.TraceInformation(e.Message);
                    }
                    sleepTime = e.RetryAfter;
                }
                catch (AggregateException ae)
                {
                    if (!(ae.InnerException is DocumentClientException))
                    {
                        Trace.TraceInformation(ae.Message);
                    }

                    DocumentClientException e = (DocumentClientException) ae.InnerException;
                    if (e.StatusCode != null && (int) e.StatusCode != 429)
                    {
                        Trace.TraceInformation(e.Message);
                    }
                    sleepTime = e.RetryAfter;
                }
                retryTimes--;
                await Task.Delay(sleepTime);
            }
            return null;
        }

        public RetryPolicy<StorageTransientErrorDetectionStrategy> GetRetryPolicy()
        {
            ExponentialBackoff retryStrategy = new ExponentialBackoff(5, TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(2));

            RetryPolicy<StorageTransientErrorDetectionStrategy> retryPolicy =
                new RetryPolicy<StorageTransientErrorDetectionStrategy>(retryStrategy);
            retryPolicy.Retrying += (sender, args) =>
            {
                // Log details of the retry.
                var msg = String.Format("Retry - Count:{0}, Delay:{1}, Exception:{2}",
                    args.CurrentRetryCount, args.Delay, args.LastException);
                Trace.TraceInformation(msg);
            };
            return retryPolicy;
        }


        public async Task<int> BatchTransfer(string sp1, string sp2, DocumentClient client, List<dynamic> docs)
        {
            try
            {
                var res = await ExecuteWithRetries(() => client.ExecuteStoredProcedureAsync<List<Document>>(
                    sp1, docs));

                var res2 = await ExecuteWithRetries(() => client.ExecuteStoredProcedureAsync<List<Document>>(
                    sp2, res.Response));

                return res2.Response.Count;
            }
            catch (Exception e)
            {
                var ee = e;
            }
            return 0;
        }

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

        public async Task CollectionTransfer(DocumentClient client, DocumentCollection dc1, DocumentCollection dc2)
        {
            var sp = client.CreateStoredProcedureQuery(dc1.SelfLink).Where(c => c.Id == "BatchDelete")
                .AsEnumerable()
                .FirstOrDefault() ?? await client.CreateStoredProcedureAsync(dc1.SelfLink, BatchDelete());

            var sp2 = client.CreateStoredProcedureQuery(dc2.SelfLink).Where(c => c.Id == "BatchInsert")
                .AsEnumerable()
                .FirstOrDefault() ?? await client.CreateStoredProcedureAsync(dc2.SelfLink, BatchInsert());

            var families =
                from f in client.CreateDocumentQuery<PostMessage>(dc1.DocumentsLink)
                where f.Type == "Post"
                select f;

            List<dynamic> d = new List<dynamic>();
            var l = families.ToList();
            var cur = 0;
            int maxDoc = 400;
            while (cur < l.Count)
            {
                List<dynamic> s = new List<dynamic>();
                for (int i = cur; i < l.Count; i++)
                {
                    if (s.Count < maxDoc)
                    {
                        s.Add(l[i]);
                    }
                }
                var n = await BatchTransfer(sp2.SelfLink, sp.SelfLink, client, s);
                Console.WriteLine(n + "----" + l.Count);
                cur = cur + n;
            }
        }


        public RangePartitionResolver<long> GetResolver(DocumentClient client, DocumentCollection dc)
        {
            var q =
                client.CreateDocumentQuery(dc.DocumentsLink)
                    .Where(x => x.Id == "AZresolver")
                    .AsEnumerable()
                    .FirstOrDefault();
            if (q != null)
            {
                var map = new Dictionary<Range<long>, string>();
                dynamic d = q;
                foreach (var dd in d.resolver.PartitionMap)
                {
                    string dz = dd.Name.ToString();
                    string[] l = dz.Split(',');
                    map.Add(new Range<long>(Convert.ToInt64(l[0]), Convert.ToInt64(l[1])), dd.Value.ToString());
                }
                RangePartitionResolver<long> rangeResolver = new RangePartitionResolver<long>(
                    u => ((PostMessage) u).Info.timestamp, map);
                return rangeResolver;
            }
            return null;
        }


        public async Task<bool> UpdateResolver(DocumentClient client, DocumentCollection dc, DocumentCollection newDc)
        {
            var oldResolver = GetResolver(client, dc);
            if (oldResolver == null) return false;

            var map = new Dictionary<Range<long>, string>();
            IDictionary<Range<long>, string> vs = oldResolver.PartitionMap;
            if (vs.Count > 1)
            {
                foreach (var v in vs)
                {
                    if (map.Count < vs.Count - 1)
                    {
                        map.Add(new Range<long>(v.Key.Low, v.Key.High), v.Value);
                    }
                }
            }
            var now = (long) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            if (now < vs.LastOrDefault().Key.Low || now > vs.LastOrDefault().Key.High) return false;

            map.Add(new Range<long>(vs.LastOrDefault().Key.Low, now), vs.LastOrDefault().Value);
            map.Add(new Range<long>(now + 1, vs.LastOrDefault().Key.High), newDc.SelfLink);
            RangePartitionResolver<long> newResolver = new RangePartitionResolver<long>(
                u => ((PostMessage) u).Info.timestamp,
                map);


            var m =
                client.CreateDocumentQuery(dc.DocumentsLink)
                    .Where(x => x.Id == "AZresolver")
                    .AsEnumerable()
                    .FirstOrDefault();
            if (m != null)
            {
                await ExecuteWithRetries(() => client.DeleteDocumentAsync(m.SelfLink));
                var res = await ExecuteWithRetries(() => client.CreateDocumentAsync(dc.SelfLink, new RangeResolver
                {
                    id = "AZresolver",
                    resolver = newResolver
                }));

                if (res.StatusCode == HttpStatusCode.Created)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> InitResolver(DocumentClient client, DocumentCollection dc)
        {

            var start = (long) (DateTime.UtcNow.AddDays(-1).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            //var end = (long) (DateTime.UtcNow.AddDays(1).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            long end = (long) (start + TimeSpan.FromDays(365).TotalMilliseconds);
            RangePartitionResolver<long> rangeResolver = new RangePartitionResolver<long>(
                u => ((PostMessage) u).Info.timestamp,
                new Dictionary<Range<long>, string>()
                {
                    {new Range<long>(start, end), dc.SelfLink}
                });
            var res = await ExecuteWithRetries(() => client.CreateDocumentAsync(dc.SelfLink, new RangeResolver
            {
                id = "AZresolver",
                resolver = rangeResolver
            }));

            if (res.StatusCode == HttpStatusCode.Created)
            {
                return true;
            }
            return false;
        }
    }
}