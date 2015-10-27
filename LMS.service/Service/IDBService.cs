using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FireSharp.Interfaces;
using LMS.model.Models;
using LMS.model.Models.Api;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Partitioning;
using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.AzureStorage;
using Microsoft.Practices.TransientFaultHandling;

namespace LMS.service.Service
{
    public interface IDbService
    {
        LMSresult GetList(string m);
        LMSresult GetMoreList(string m, long start);
        IFirebaseClient GetFirebaseClient();
        IFirebaseClient GetFirebaseClient(string node);
        DocumentCollection GetDc(string cName, string dName);
        Database GetDd(string dName);
        DocumentCollection GetCurrentDc();
        StoredProcedure GetSp(DocumentCollection documentCollection, string spName);
        DocumentClient GetDocumentClient();
        DocumentClient GetDocumentClient(bool t);
        void UpdateDocumentClient();
        List<Topic> GetCalendar();
        String GetFirebaseToken(string user, string uid, string data);
        DocumentCollection SearchCollection(string dis, DocumentCollection masterCollection, Database database);
        PostMessage PostData(dynamic x, string[] path);
        TableChat TableChatData(dynamic u, dynamic s);
        TablePost TablePostData(dynamic post);
        Task DeleteDocByIdList(DocumentCollection dc, List<string> idList);
        Task DeleteDocById(DocumentCollection dc, string id);
        Task ReplaceDocument(dynamic item);

        Task ExecuteWithRetries(Func<object> function);

        Task<ResourceResponse<Document>> ExecuteWithRetries(
            Func<Task<ResourceResponse<Document>>> function);

        Task ExecuteWithRetries(Func<Task> function);

        Task<ResourceResponse<Document>> ExecuteWithRetries(int retryTimes,
            Func<Task<ResourceResponse<Document>>> function);

        Task ExecuteWithRetries(int retryTimes, Func<Task> function);


        RetryPolicy<StorageTransientErrorDetectionStrategy> GetRetryPolicy();
        Task<int> BatchTransfer(string sp1, string sp2, List<dynamic> docs);
        Task BatchDelete(DocumentCollection dc, List<dynamic> docs);
        Task CollectionTransfer(DocumentCollection dc1, DocumentCollection dc2);
        RangePartitionResolver<long> GetResolver(DocumentClient client);
        Task<bool> UpdateResolver(DocumentCollection newDc);
        Task<bool> InitResolver(string link);
        Task UpdateCurrentCollection(DocumentCollection newDc);
    }
}