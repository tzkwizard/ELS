using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using LMS.Common.DAL;
using LMS.Common.Models;
using LMS.Common.Models.Api;
using LMS.Common.Service.Interface;
using Microsoft.Azure;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents.Partitioning;
using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.AzureStorage;
using Microsoft.Practices.TransientFaultHandling;

namespace LMS.Common.Service
{
    public class DbService : IDbService
    {
        private static IDBoperation _iDBoperation;
        private static IFBoperation _iFBoperation;
        private static IResolverService _iResolverService;

        #region Constructor

        public DbService(IDBoperation iDBoperation, IRetryService iRetryService, IFBoperation iFBoperation,
            IResolverService iResolverService)
        {
            _iFBoperation = iFBoperation;
            _iDBoperation = iDBoperation;
            _iResolverService = iResolverService;
        }

        public DbService()
        {
            _iDBoperation = new DBoperation();
            _iFBoperation = new FBoperation();
            _iResolverService = new ResolverService();
        }

        #endregion

        #region Api

        public List<Topic> GetCalendar()
        {
            var client = GetDocumentClient();
            var dataSelfLink = CloudConfigurationManager.GetSetting("DBSelfLink") ??
                               ConfigurationManager.AppSettings["DBSelfLink"];
            var items =
                client.CreateDocumentQuery<Topic>(dataSelfLink,
                    new FeedOptions {MaxItemCount = 100})
                    .Where(f => f.Type == "Topic")
                    .OrderBy(o => o.Info.due).ToList();
            return items.ToList();
        }

        public LMSresult GetList(string m)
        {
            var client = GetDocumentClient(true);
            long end = 0;
            var n = 0;
            List<PostMessage> items = new List<PostMessage>();
            while (items.Count < 5 && n < 6)
            {
                end =
                    (long) (DateTime.UtcNow.AddMinutes(-30*n).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
                var path = m.Split('/');
                /*   items = client.CreateDocumentQuery<PostMessage>(documentCollection.DocumentsLink,
                    "SELECT d AS data " +
                    "FROM Doc d " +
                    "Where d.Type='Post' And d.Info.timestamp > '" + time + "'").OrderBy(o=>o.Info.timestamp).ToList();*/
                /*  items =
                    (from f in client.CreateDocumentQuery<PostMessage>(_database.SelfLink)
                        where f.Type == "Post" && f.Info.timestamp > end
                        select f).OrderBy(o => o.Info.timestamp).ToList();*/
                var dataSelfLink = CloudConfigurationManager.GetSetting("DBSelfLink") ??
                                   ConfigurationManager.AppSettings["DBSelfLink"];
                items =
                    client.CreateDocumentQuery<PostMessage>(dataSelfLink,
                        new FeedOptions {MaxItemCount = 2000})
                        .Where(f => f.Type == "Post" && f.Info.timestamp > end)
                        .OrderBy(o => o.Info.timestamp).ToList();
                n++;
            }

            var res = new LMSresult
            {
                time = end,
                list = items
            };
            return res;
        }

        public LMSresult GetMoreList(string m, long start)
        {
            var client = GetDocumentClient(true);
            var t = (long) (DateTime.UtcNow.AddMonths(-1).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            List<PostMessage> items = new List<PostMessage>();
            var t1 = DateTime.Now;
            var n = 1;
            var i = 1;
            var end = start;
            while (items.Count < 5 && end > t)
            {
                i = i + n;
                end = end - (long) TimeSpan.FromHours(i).TotalMilliseconds;
                var path = m.Split('/');
                /*  items = client.CreateDocumentQuery<PostMessage>(documentCollection.DocumentsLink,
                    "SELECT d AS data " +
                    "FROM Doc d " +
                    "Where d.Type='Post' And d.Info.timestamp > '" + t2 + "'" +
                    "And d.Info.timestamp < '" + t1 + "'").ToList();*/
                /*  items =
                    (from f in client.CreateDocumentQuery<PostMessage>(_database.SelfLink)
                        where f.Type == "Post" && f.Info.timestamp < start && f.Info.timestamp > end
                        select f).OrderBy(o => o.Info.timestamp).ToList();*/
                var dataSelfLink = CloudConfigurationManager.GetSetting("DBSelfLink") ??
                                   ConfigurationManager.AppSettings["DBSelfLink"];
                items = client.CreateDocumentQuery<PostMessage>(dataSelfLink)
                    .Where(f => f.Type == "Post" && f.Info.timestamp < start && f.Info.timestamp > end)
                    .OrderBy(o => o.Info.timestamp).ToList();
                var t2 = DateTime.Now;
                if (t2 - t1 > TimeSpan.FromSeconds(10))
                {
                    return new LMSresult
                    {
                        moreData = true,
                        time = end,
                        list = items
                    };
                }
                n++;
            }
            var res = new LMSresult
            {
                moreData = false,
                time = end,
                list = items
            };
            return res;
        }

        #endregion

        #region Client

        public IFirebaseClient GetFirebaseClient()
        {
            return _iFBoperation.GetFirebaseClient();
        }

        public DocumentClient GetDocumentClient()
        {
            return _iDBoperation.GetDocumentClient();
        }

        public DocumentClient GetDocumentClient(bool force)
        {
            UpdateDocumentClient();
            return _iDBoperation.GetDocumentClient();
        }

        public void UpdateDocumentClient()
        {
            var rangeResolver = _iResolverService.GetResolver();
            _iDBoperation.UpdateDocumentClient(rangeResolver);
        }

        #endregion

        #region misc

        public async Task CollectionTransfer(DocumentCollection dc1, DocumentCollection dc2)
        {
            var client = GetDocumentClient();
            var sp = await _iDBoperation.GetStoreProcedure(dc1.SelfLink, "BatchDelete");
            var sp2 = await _iDBoperation.GetStoreProcedure(dc1.SelfLink, "BatchInsert");

            var docs = _iDBoperation.GetDocumentByType(dc1.DocumentsLink, "Post");

            var d = new List<dynamic>();
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
            var client = GetDocumentClient();
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
            await _iDBoperation.CreateDocument("Default", doc);
        }

        #endregion

        #region Interface

        public IResolverService RangePartitionResolver()
        {
            return _iResolverService;
        }

        public IDBoperation DBoperation()
        {
            return _iDBoperation;
        }

        public IFBoperation FBoperation()
        {
            return _iFBoperation;
        }

        #endregion
    }
}