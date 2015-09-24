using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using MessageHandleApi.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Nest;
using StackExchange.Redis;

namespace MessageHandleApi.Service
{
    public class DBService : IDBService
    {
        private static string EndpointUrl = "https://tstaz.documents.azure.com:443/";

        private static string AuthorizationKey =
            "6xPkxpC7FyiozobQOtQ8yFxbqd7uLOCz0pRo4i+GKxHdmISxDrMKZdaKQH0/0BJe/xC3UKdQM4C1x5d4Rxk3AQ==";

        private readonly DocumentClient _documentClient;


        public DBService()
        {
            _documentClient = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
        }

        public List<dynamic> GetList(string m)
        {
            var time = (long) (DateTime.UtcNow.AddHours(-6).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            var documentCollection = GetDc(_documentClient, "LMSCollection", "LMSRegistry");
            var path = m.Split('/');
            var items = _documentClient.CreateDocumentQuery<dynamic>(documentCollection.DocumentsLink,
                "SELECT d AS data " +
                "FROM Doc d " +
                "Where d.Type='Post' And d.Info.timestamp > '" + time + "'");

            return items.ToList();
        }

        public List<dynamic> GetMoreList(string m, int start, int end)
        {            
            var t1 = (long) (DateTime.UtcNow.AddHours(-6*start).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            var t2 = (long) (DateTime.UtcNow.AddHours(-6*end).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            var documentCollection = GetDc(_documentClient, "LMSCollection", "LMSRegistry");
            var path = m.Split('/');
            var items = _documentClient.CreateDocumentQuery<dynamic>(documentCollection.DocumentsLink,
                "SELECT d AS data " +
                "FROM Doc d " +
                "Where d.Type='Post' And d.Info.timestamp > '" + t2 + "'" +
                "And d.Info.timestamp < '" + t1 + "'");

            return items.ToList();
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

        public StoredProcedure GetSp(DocumentClient client, DocumentCollection documentCollection,string spName)
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

        public DocumentClient GetDocumentClient()
        {
            var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
            return client;
        }

    }
}