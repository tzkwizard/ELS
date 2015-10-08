using System;
using System.Collections.Generic;
using System.Linq;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
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

        public LMSresult GetList(string m)
        {
           /* var time = (long) (DateTime.UtcNow.AddHours(-6).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            var documentCollection = GetDc(_documentClient, "LMSCollection", "LMSRegistry");
            var path = m.Split('/');
            var items = _documentClient.CreateDocumentQuery<dynamic>(documentCollection.DocumentsLink,
                "SELECT d AS data " +
                "FROM Doc d " +
                "Where d.Type='Post' And d.Info.timestamp > '" + time + "'");
*/
            var end = 1;
            var n = 0;
            List<dynamic> items = new List<dynamic>();
            while (items.Count < 5 && end < 5)
            {
                end = end + n;
                var time = (long)(DateTime.UtcNow.AddHours(-6 * end).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
                var documentCollection = GetDc(_documentClient, "LMSCollection", "LMSRegistry");
                var path = m.Split('/');
                items = _documentClient.CreateDocumentQuery<dynamic>(documentCollection.DocumentsLink,
                "SELECT d AS data " +
                "FROM Doc d " +
                "Where d.Type='Post' And d.Info.timestamp > '" + time + "'").ToList();
                n++;
            }


            var res = new LMSresult
            {
                time = end,
                list = items
            };
            return res;
        }

        public LMSresult GetMoreList(string m, int start)
        {
            var t1 = (long) (DateTime.UtcNow.AddHours(-6*start).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            List<dynamic> items = new List<dynamic>();
            var end = start + 1;
            var n = 0;
            while (items.Count < 5 && end < (MaxMonthTime*4*30))
            {
                end = end + n;
                var t2 = (long) (DateTime.UtcNow.AddHours(-6*end).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
                var documentCollection = GetDc(_documentClient, "LMSCollection", "LMSRegistry");
                var path = m.Split('/');
                items = _documentClient.CreateDocumentQuery<dynamic>(documentCollection.DocumentsLink,
                    "SELECT d AS data " +
                    "FROM Doc d " +
                    "Where d.Type='Post' And d.Info.timestamp > '" + t2 + "'" +
                    "And d.Info.timestamp < '" + t1 + "'").ToList();
                n++;
            }
            var res = new LMSresult
            {
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
    }
}