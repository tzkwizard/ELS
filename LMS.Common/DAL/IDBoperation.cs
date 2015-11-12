using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LMS.Common.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Partitioning;

namespace LMS.Common.DAL
{
    public interface IDBoperation
    {
        DocumentCollection GetDc(string cName, string dName);
        Database GetDd(string dName);
        DocumentCollection GetCurrentDc();
        DocumentClient GetDocumentClient();
        void UpdateDbClientResolver(RangePartitionResolver<long> rangeResolver);
        Task DeleteDocByIdList(DocumentCollection dc, List<string> idList);
        Task DeleteDocById(string selfLink, string id);
        Task<bool> CreateDocument(object doc);
        Task<bool> CreateDocument(string selfLink, object doc);
        Task ReplaceDocument(dynamic item);
        Task<StoredProcedure> GetStoreProcedure(string dcLink, string spName);
        Task BatchDelete(DocumentCollection dc, List<dynamic> docs);
        Document GetDocumentById(string dcLink, string id);
        dynamic GetDocumentByType(string dcLink, string type);

        List<PostMessage> GetPostMessages(long start, long end);
        List<PostMessage> GetPostMessages(long end);
    }
}