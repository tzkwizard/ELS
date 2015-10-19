using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FireSharp.Interfaces;
using LMS.model.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Partitioning;
using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.AzureStorage;
using Microsoft.Practices.TransientFaultHandling;

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
        String GetFirebaseToken(string user, string uid, string data);
        DocumentCollection SearchCollection(string dis, DocumentCollection masterCollection, Database database);
        PostMessage PostData(dynamic x, string[] path);
        TableChat TableChatData(dynamic u, dynamic s);
        TablePost TablePostData(dynamic post);
        Task DeleteDocByIdList(DocumentClient client, DocumentCollection dc, List<string> idList, int retryTimes);
        Task DeleteDocById(DocumentClient client, DocumentCollection dc, string id, int retryTimes);

        Task DeleteDocument(DocumentClient client, string selfLink, int retryTimes);
        Task ReplaceDocument(DocumentClient client, dynamic item, int retryTimes);

        Task<ResourceResponse<Document>> AddDocument(DocumentClient client, string dcSelfLink, dynamic item,
            int retryTimes);

        Task<ResourceResponse<Document>> ExecuteWithRetries(
            Func<Task<ResourceResponse<Document>>> function);

        Task ExecuteWithRetries(Func<Task> function);

        Task<ResourceResponse<Document>> ExecuteWithRetries(int retryTimes,
            Func<Task<ResourceResponse<Document>>> function);

        Task ExecuteWithRetries(int retryTimes, Func<Task> function);
        RetryPolicy<StorageTransientErrorDetectionStrategy> GetRetryPolicy();

        Task<int> BatchTransfer(string sp1, string sp2, DocumentClient client, List<dynamic> docs);
        Task BatchDelete(DocumentCollection dc, DocumentClient client, List<dynamic> docs);
        Task CollectionTransfer(DocumentClient client, DocumentCollection dc1, DocumentCollection dc2);

        RangePartitionResolver<long> GetResolver(DocumentClient client, DocumentCollection dc);

        Task<bool> UpdateResolver(DocumentClient client, DocumentCollection dc, DocumentCollection newDc);
        Task<bool> InitResolver(DocumentClient client, DocumentCollection dc);
    }
}