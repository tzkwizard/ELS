using System.Collections.Generic;
using System.Threading.Tasks;
using FireSharp.Interfaces;
using LMS.Common.DAL;
using LMS.Common.Models;
using LMS.Common.Models.Api;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace LMS.Common.Service.Interface
{
    public interface IDbService
    {
        LMSresult GetList(string m);
        LMSresult GetMoreList(string m, long start);
        IFirebaseClient GetFirebaseClient();
        DocumentClient GetDocumentClient();
        DocumentClient GetDocumentClient(bool t);
        void UpdateDocumentClient();
        List<Topic> GetCalendar();
        Task CollectionTransfer(DocumentCollection dc1, DocumentCollection dc2);
        Task UpdateCurrentCollection(DocumentCollection newDc);
        IResolverService RangePartitionResolver();
        IDBoperation DBoperation();
        IFBoperation FBoperation();
    }
}