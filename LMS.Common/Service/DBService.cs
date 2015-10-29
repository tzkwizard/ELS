using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using LMS.Common.Models;
using LMS.Common.Models.Api;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents.Partitioning;
using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.AzureStorage;
using Microsoft.Practices.TransientFaultHandling;

namespace LMS.Common.Service
{
    public class DbService : IDbService
    {
        private static DocumentClient _documentClient;

        private const int RetryTimes = 4;
        private static string _endpointUrl;
        private static string _authorizationKey;
        private static string _dataSelfLink;
        private static string _masterCollectionSelfLink;
        private static string _firebaseSecret;
        public DbService(string endpointUrl, string authorizationKey)
        {
            _endpointUrl = endpointUrl;
            _authorizationKey = authorizationKey;
            _dataSelfLink = ConfigurationManager.AppSettings["DBSelfLink"];
            _masterCollectionSelfLink = ConfigurationManager.AppSettings["MasterCollectionSelfLink"];
            _firebaseSecret = ConfigurationManager.AppSettings["FirebaseSecret"];
            var t = ConfigurationManager.ConnectionStrings["AzureRedis"].ToString();
        }

        public DbService()
        {
            _masterCollectionSelfLink = ConfigurationManager.AppSettings["MasterCollectionSelfLink"];
            _dataSelfLink = ConfigurationManager.AppSettings["DBSelfLink"];
            _endpointUrl = ConfigurationManager.AppSettings["DBEndpointUrl"];
            _authorizationKey = ConfigurationManager.AppSettings["DBReadOnlyKey"];
            _firebaseSecret = ConfigurationManager.AppSettings["FirebaseSecret"];
        }

        public List<Topic> GetCalendar()
        {
            var client = GetDocumentClient();
            var documentCollection = GetDc("LMSCollection", "LMSRegistry");
            var item =
                from f in client.CreateDocumentQuery<Topic>(documentCollection.DocumentsLink)
                where f.Type == "Topic"
                select f;
            return item.ToList();
        }

        public string GetFirebaseToken(string user, string uid, string data)
        {
            var tokenGenerator = new Firebase.TokenGenerator(_firebaseSecret);
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
            var client = GetDocumentClient(true);
            long end = 0;
            var n = 0;
            List<PostMessage> items = new List<PostMessage>();
            while (items.Count < 5 && n < 6)
            {
                end =
                    (long) (DateTime.UtcNow.AddMinutes(-30*n).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
                var path = m.Split('/');
                /*   items = client.CreateDocumentQuery<PostMessage>(documentCollection.DocumentsLink,
                    "SELECT d AS data " +
                    "FROM Doc d " +
                    "Where d.Type='Post' And d.Info.timestamp > '" + time + "'").OrderBy(o=>o.Info.timestamp).ToList();*/
                /*  items =
                    (from f in client.CreateDocumentQuery<PostMessage>(_database.SelfLink)
                        where f.Type == "Post" && f.Info.timestamp > end
                        select f).OrderBy(o => o.Info.timestamp).ToList();*/
                items =
                    client.CreateDocumentQuery<PostMessage>(_dataSelfLink,
                        new FeedOptions {MaxItemCount = 2000})
                        .Where(f => f.Type == "Post" && f.Info.timestamp > end)
                        .OrderBy(o => o.Info.timestamp).ToList();
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
            var client = GetDocumentClient(true);
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
                var path = m.Split('/');
                /*  items = client.CreateDocumentQuery<PostMessage>(documentCollection.DocumentsLink,
                    "SELECT d AS data " +
                    "FROM Doc d " +
                    "Where d.Type='Post' And d.Info.timestamp > '" + t2 + "'" +
                    "And d.Info.timestamp < '" + t1 + "'").ToList();*/
                /*  items =
                    (from f in client.CreateDocumentQuery<PostMessage>(_database.SelfLink)
                        where f.Type == "Post" && f.Info.timestamp < start && f.Info.timestamp > end
                        select f).OrderBy(o => o.Info.timestamp).ToList();*/

                items = client.CreateDocumentQuery<PostMessage>(_dataSelfLink)
                    .Where(f => f.Type == "Post" && f.Info.timestamp < start && f.Info.timestamp > end)
                    .OrderBy(o => o.Info.timestamp).ToList();
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

        public StoredProcedure GetSp(DocumentCollection documentCollection, string spName)
        {
            var client = GetDocumentClient();
            var sp = client.CreateStoredProcedureQuery(documentCollection.SelfLink).Where(c => c.Id == spName)
                .AsEnumerable()
                .FirstOrDefault();
            return sp;
        }

        public IFirebaseClient GetFirebaseClient()
        {
            var node = "https://dazzling-inferno-4653.firebaseio.com/";
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = _firebaseSecret,
                BasePath = node
            };
            IFirebaseClient client = new FirebaseClient(config);
            return client;
        }

        public IFirebaseClient GetFirebaseClient(string node)
        {
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = _firebaseSecret,
                BasePath = node
            };
            IFirebaseClient client = new FirebaseClient(config);
            return client;
        }


        public DocumentClient GetDocumentClient()
        {
            return _documentClient ?? (_documentClient = new DocumentClient(new Uri(_endpointUrl), _authorizationKey));
        }

        public DocumentClient GetDocumentClient(bool t)
        {
            UpdateDocumentClient();
            return _documentClient ?? (_documentClient = new DocumentClient(new Uri(_endpointUrl), _authorizationKey));
        }

        public void UpdateDocumentClient()
        {
            _documentClient = _documentClient ?? new DocumentClient(new Uri(_endpointUrl), _authorizationKey);
            var rangeResolver = GetResolver(_documentClient);
            _documentClient.PartitionResolvers[_dataSelfLink] = rangeResolver;
        }

        public DocumentCollection SearchCollection(string dis, DocumentCollection masterCollection, Database database)
        {
            var client = GetDocumentClient();
            var ds =
                from d in client.CreateDocumentQuery<DcAllocate>(masterCollection.DocumentsLink)
                where d.Type == "DisList"
                select d;
            foreach (var d in ds)
            {
                if (d.District.Contains(dis))
                {
                    DocumentCollection dc = client.CreateDocumentCollectionQuery(database.SelfLink)
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


        public async Task DeleteDocByIdList(DocumentCollection dc, List<string> idList)
        {
            foreach (var id in idList)
            {
                await DeleteDocById(dc, id);
            }
        }

        public async Task DeleteDocById(DocumentCollection dc, string id)
        {
            var client = GetDocumentClient();
            var doc =
                (from d in client.CreateDocumentQuery(dc.DocumentsLink)
                    where d.Id == id
                    select d).AsEnumerable().FirstOrDefault();
            if (doc != null)
            {
                await ExecuteWithRetries(() => client.DeleteDocumentAsync(doc.SelfLink));
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


        public async Task ReplaceDocument(dynamic item)
        {
            var client = GetDocumentClient();
            await ExecuteWithRetries(() => client.ReplaceDocumentAsync(item));
        }

        public async Task ExecuteWithRetries(Func<object> function)
        {
            TimeSpan sleepTime = TimeSpan.FromMilliseconds(100);
            var retryTimes = RetryTimes;
            while (retryTimes > 0 && sleepTime != TimeSpan.Zero)
            {
                try
                {
                    function();
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


        public async Task<int> BatchTransfer(string sp1, string sp2, List<dynamic> docs)
        {
            var client = GetDocumentClient();
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

        public async Task BatchDelete(DocumentCollection dc, List<dynamic> docs)
        {
            var client = GetDocumentClient();
            var sp = client.CreateStoredProcedureQuery(dc.SelfLink).Where(c => c.Id == "BatchDelete")
                .AsEnumerable()
                .FirstOrDefault() ?? await client.CreateStoredProcedureAsync(dc.SelfLink, BatchDelete());
            var res = await ExecuteWithRetries(() => client.ExecuteStoredProcedureAsync<List<Document>>(
                sp.SelfLink, docs));


            //Process if some delete fail in batchDelete 
            if (res.Response.Count != docs.Count)
            {
                var cur = res.Response.Count;
                while (cur < docs.Count)
                {
                    var s = new List<dynamic>();
                    for (int i = cur; i < docs.Count; i++)
                    {
                        if (s.Count < docs.Count)
                        {
                            s.Add(docs[i]);
                        }
                    }
                    var res2 =
                        await ExecuteWithRetries(() => client.ExecuteStoredProcedureAsync<List<Document>>(
                            sp.SelfLink, docs));
                    cur = cur + res2.Response.Count;
                }
            }
        }

        public async Task CollectionTransfer(DocumentCollection dc1, DocumentCollection dc2)
        {
            var client = GetDocumentClient();
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

            var d = new List<dynamic>();
            var l = families.ToList();
            var cur = 0;
            int maxDoc = 400;
            while (cur < l.Count)
            {
                var s = new List<dynamic>();
                for (var i = cur; i < l.Count; i++)
                {
                    if (s.Count < maxDoc)
                    {
                        s.Add(l[i]);
                    }
                }
                var n = await BatchTransfer(sp2.SelfLink, sp.SelfLink, s);
                Console.WriteLine(n + "----" + l.Count);
                cur = cur + n;
            }
        }

        public RangePartitionResolver<long> GetResolver(DocumentClient client)
        {
            var q =
                client.CreateDocumentQuery(_masterCollectionSelfLink)
                    .Where(x => x.Id == "AZresolver")
                    .AsEnumerable()
                    .FirstOrDefault();
            if (q == null) return null;
            var map = new Dictionary<Range<long>, string>();
            dynamic d = q;
            foreach (var dd in d.resolver.PartitionMap)
            {
                string dz = dd.Name.ToString();
                var l = dz.Split(',');
                map.Add(new Range<long>(Convert.ToInt64(l[0]), Convert.ToInt64(l[1])), dd.Value.ToString());
            }
            var rangeResolver = new RangePartitionResolver<long>(
                u => ((PostMessage) u).Info.timestamp, map);
            return rangeResolver;
        }

        public async Task<bool> UpdateResolver(DocumentCollection newDc)
        {
            var client = GetDocumentClient();
            var oldResolver = GetResolver(client);
            if (oldResolver == null) return false;

            var map = new Dictionary<Range<long>, string>();
            var vs = oldResolver.PartitionMap;
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
            var newResolver = new RangePartitionResolver<long>(
                u => ((PostMessage) u).Info.timestamp,
                map);


            var m =
                client.CreateDocumentQuery(_masterCollectionSelfLink)
                    .Where(x => x.Id == "AZresolver")
                    .AsEnumerable()
                    .FirstOrDefault();
            if (m == null) return false;
            await ExecuteWithRetries(() => client.DeleteDocumentAsync(m.SelfLink));
            var res =
                await
                    ExecuteWithRetries(
                        () => client.CreateDocumentAsync(_masterCollectionSelfLink, new RangeResolver
                        {
                            id = "AZresolver",
                            resolver = newResolver
                        }));

            return res.StatusCode == HttpStatusCode.Created;
        }

        public async Task<bool> InitResolver(string selfLink)
        {
            var masterCollectionSelfLink = _masterCollectionSelfLink;
            if (selfLink != "")
            {
                masterCollectionSelfLink = selfLink;
            }
        
            var client = GetDocumentClient();
            var start = (long) (DateTime.UtcNow.AddDays(-1).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            var end = (long) (start + TimeSpan.FromDays(365).TotalMilliseconds);
            var rangeResolver = new RangePartitionResolver<long>(
                u => ((PostMessage) u).Info.timestamp,
                new Dictionary<Range<long>, string>()
                {
                    {new Range<long>(start, end), masterCollectionSelfLink}
                });

            var m =
                client.CreateDocumentQuery(masterCollectionSelfLink)
                    .Where(x => x.Id == "AZresolver")
                    .AsEnumerable()
                    .FirstOrDefault();
            if (m != null)
            {
                await ExecuteWithRetries(() => client.DeleteDocumentAsync(m.SelfLink));
            }
            var res =
                await
                    ExecuteWithRetries(
                        () => client.CreateDocumentAsync(masterCollectionSelfLink, new RangeResolver
                        {
                            id = "AZresolver",
                            resolver = rangeResolver
                        }));

            return res.StatusCode == HttpStatusCode.Created;
        }

        public async Task UpdateCurrentCollection(DocumentCollection newDc)
        {
            var client = GetDocumentClient();
            await ExecuteWithRetries(() => client.DeleteDocumentAsync(_masterCollectionSelfLink));
            await
                ExecuteWithRetries(
                    () => client.CreateDocumentAsync(_masterCollectionSelfLink, new CurrentCollection
                    {
                        id = "CurrentCollection",
                        name = newDc.Id
                    }));
        }
    }
}