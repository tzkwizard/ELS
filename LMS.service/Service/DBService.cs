using System;
using System.Collections.Generic;
using System.Linq;
using FireSharp;
using FireSharp.Interfaces;
using FireSharp.Config;
using LMS.model.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

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
                            where f.Type == "Post" &&  f.Info.timestamp > end
                            select f).OrderBy(o => o.Info.timestamp).ToList();
                n++;
            }

            var res = new LMSresult
            {
                moreData = true,
                time = end,
                list = items
            };
            return res;
        }

        public LMSresult GetMoreList(string m, long start)
        {
            var t = (long) (DateTime.UtcNow.AddMonths(-1).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            List<PostMessage> items = new List<PostMessage>();
            var t1=DateTime.Now;
            var n = 1;
            var i = 1;
            var end = start;
            while (items.Count < 5 && end >t)
            {
                i = i + n;
                end = end-(long)TimeSpan.FromHours(i).TotalMilliseconds;
                var documentCollection = GetDc(_documentClient, "LMSCollection", "LMSRegistry");
                var path = m.Split('/');
              /*  items = _documentClient.CreateDocumentQuery<PostMessage>(documentCollection.DocumentsLink,
                    "SELECT d AS data " +
                    "FROM Doc d " +
                    "Where d.Type='Post' And d.Info.timestamp > '" + t2 + "'" +
                    "And d.Info.timestamp < '" + t1 + "'").ToList();*/
                items =
                   (from f in _documentClient.CreateDocumentQuery<PostMessage>(documentCollection.DocumentsLink)
                    where f.Type == "Post" && f.Info.timestamp < start && f.Info.timestamp >end
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
    }
}