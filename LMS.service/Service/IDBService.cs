using System;
using System.Collections.Generic;
using FireSharp.Interfaces;
using LMS.model.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace LMS.service.Service
{
    public interface IDBService
    {
         LMSresult GetList(string m);
         LMSresult GetMoreList(string m, long start);
         IFirebaseClient GetFirebaseClient();
         IFirebaseClient GetFirebaseClient(string node);
         DocumentCollection GetDc(DocumentClient client, string cName, string dName);
         Database GetDd(DocumentClient client, string dName);
         StoredProcedure GetSp(DocumentClient client, DocumentCollection documentCollection, string spName);
         DocumentClient GetDocumentClient();
         List<Topic> GetCalendar();
         String GetFirebaseToken(string user,string uid,string data);
         DocumentCollection SearchCollection(string dis, DocumentCollection masterCollection, Database database);
         PostMessage PostData(dynamic x, string[] path);
         TableChat TableChatData(dynamic u, dynamic s);
    }
}