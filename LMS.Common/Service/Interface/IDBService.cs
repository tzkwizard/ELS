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
        IFirebaseClient GetFirebaseClient();
        DocumentClient GetDocumentClient();
        DocumentClient GetDocumentClient(bool t);
        void UpdateDocumentClient();
        IResolverService RangePartitionResolver();
        IDBoperation DBoperation();
        IFBoperation FBoperation();
        ICollectionService CollectionService();
        IASoperation ASoperation();
    }
}