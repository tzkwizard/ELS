using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using FireSharp.Interfaces;
using MessageHandleApi.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace MessageHandleApi.Service
{
    public interface IDBService
    {
         LMSresult GetList(string m);
         LMSresult GetMoreList(string m,int start);
         IFirebaseClient GetFirebaseClient();
         DocumentCollection GetDc(DocumentClient client, string cName, string dName);
         Database GetDd(DocumentClient client, string dName);
         StoredProcedure GetSp(DocumentClient client, DocumentCollection documentCollection, string spName);
         DocumentClient GetDocumentClient();
         List<Topic> GetCalendar();
    }
}