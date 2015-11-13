using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LMS.Common.DAL;
using LMS.Common.Models;
using LMS.Common.Service.Interface;
using Microsoft.Azure.Documents;

namespace LMS.Common.Service
{
    public class CollectionService : ICollectionService
    {
        private static IDBoperation _iDBoperation;

        public CollectionService()
        {
            _iDBoperation = new DBoperation();
        }

        public async Task CollectionTransfer(DocumentCollection dc1, DocumentCollection dc2)
        {
            var sp = await _iDBoperation.GetStoreProcedure(dc1.SelfLink, "BatchDelete");
            var sp2 = await _iDBoperation.GetStoreProcedure(dc1.SelfLink, "BatchInsert");

            var docs = _iDBoperation.GetDocumentByType(dc1.DocumentsLink, "Post");

            var l = docs.ToList();
            var cur = 0;
            int maxDoc = 400;
            while (cur < l.Count)
            {
                var s = new List<dynamic>();
                for (var i = cur; i < l.Count; i++)
                {
                    if (s.Count < maxDoc)
                    {
                        s.Add(l[i]);
                    }
                }
                var n = await BatchTransfer(sp2.SelfLink, sp.SelfLink, s);
                //Console.WriteLine(n + "----" + l.Count);
                cur = cur + n;
            }
        }

        private async Task<int> BatchTransfer(string sp1, string sp2, List<dynamic> docs)
        {
            var client = _iDBoperation.GetDocumentClient();
            try
            {
                var res =
                    await RetryService.ExecuteWithRetries(() => client.ExecuteStoredProcedureAsync<List<Document>>(
                        sp1, docs));

                var res2 =
                    await RetryService.ExecuteWithRetries(() => client.ExecuteStoredProcedureAsync<List<Document>>(
                        sp2, res.Response));

                return res2.Response.Count;
            }
            catch (Exception e)
            {
                var ee = e;
            }
            return 0;
        }

        public async Task UpdateCurrentCollection(DocumentCollection newDc)
        {
            await _iDBoperation.DeleteDocById("", "CurrentCollection");
            var doc = new CurrentCollection
            {
                id = "CurrentCollection",
                name = newDc.Id
            };
            await _iDBoperation.CreateDocument(null, doc);
        }
    }
}