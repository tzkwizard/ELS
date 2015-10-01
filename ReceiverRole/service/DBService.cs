using System;
using System.Collections.Generic;
using System.Linq;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace ReceiverRole.service
{
    public class DBService : IDBService
    {
        private static string EndpointUrl ;

        private static string AuthorizationKey;

        private readonly DocumentClient _documentClient;
        private const int MaxMonthTime = 1;

        public DBService(string endpointUrl, string authorizationKey)
        {
            EndpointUrl = endpointUrl;
            AuthorizationKey = authorizationKey;
            //_documentClient = new DocumentClient(new Uri(endpointUrl), authorizationKey);
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

        public DocumentClient GetDocumentClient()
        {
            var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);
            return client;
        }
    }
}