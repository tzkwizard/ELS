using System.Collections.Generic;
using FireSharp.Interfaces;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace ReceiverRole.service
{
    public interface IDBService
    {
         IFirebaseClient GetFirebaseClient();
         DocumentCollection GetDc(DocumentClient client, string cName, string dName);
         Database GetDd(DocumentClient client, string dName);
         StoredProcedure GetSp(DocumentClient client, DocumentCollection documentCollection, string spName);
         DocumentClient GetDocumentClient();
    }
}