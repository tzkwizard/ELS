using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LMS.model.Models;
using LMS.service.Service;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.WindowsAzure;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;

namespace CheckRole
{
    public class Dichotomy
    {
        private static DocumentClient _client;
        private static Database _database;
        private static IDBService _iDbService;
        private static int reTry = 5;

        public Dichotomy(string endpointUrl, string authorizationKey)
        {
            _client = new DocumentClient(new Uri(endpointUrl), authorizationKey);
            _database =
                _client.CreateDatabaseQuery().Where(db => db.Id == CloudConfigurationManager.GetSetting("Database"))
                    .AsEnumerable().FirstOrDefault();
            _iDbService = new DBService();
        }

        public async Task UpdateDcAll()
        {
            try
            {
                IEnumerable<DocumentCollection> dz = _client.CreateDocumentCollectionQuery(_database.SelfLink)
                    .AsEnumerable();

                DocumentCollection origin = _client.CreateDocumentCollectionQuery(_database.SelfLink)
                    .Where(c => c.Id == CloudConfigurationManager.GetSetting("MasterCollection"))
                    .AsEnumerable()
                    .FirstOrDefault();
                if (origin != null)
                {
                    foreach (var x in dz)
                    {
                        if (x.Id == "LMSCollection")
                        {
                            var x1 = x;
                            await _iDbService.ExecuteWithRetries(() => UpdateDc(x1, origin));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (reTry > 0)
                {
                    reTry--;
                    Thread.Sleep(60000);
                    Trace.TraceInformation("Restart UpdateAll... in 1 min " + reTry);
                    UpdateDcAll().Wait();
                }
                else
                {
                    Trace.TraceError("Error in start " + e.Message);
                }
            }
        }

        private static async Task UpdateDc(DocumentCollection oldDc, DocumentCollection origin)
        {
            var run = await CheckSize(oldDc, origin);
            if (run)
            {
                Trace.TraceInformation("Backup Collection Start.  Name: '{0}', Time: '{1}'", oldDc.Id,
                    DateTime.Now.ToString(CultureInfo.CurrentCulture));

                var list = GenerateList(oldDc);
                var newList = list[1];
                var oldList = list[0];

                if (newList.Count > 0)
                {
                    //create new collection
                    /*var t = (long) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
                  DocumentCollection newDc=await client.CreateDocumentCollectionAsync(database.CollectionsLink,
                                                        new DocumentCollection
                                                        {
                                                            Id = "LMSCollection"+t
                                                        });*/

                    //search collection
                    DocumentCollection newDc = _client.CreateDocumentCollectionQuery(_database.SelfLink)
                        .Where(c => c.Id == "LMSCollection1444075919174")
                        .AsEnumerable()
                        .FirstOrDefault();
                    if (newDc != null)
                    {
                        TransferDc(oldDc, newDc, newList).Wait();
                        Trace.TraceInformation("Update District List.  Name: '{0}'==>'{1}'", oldDc.Id,
                            newDc.Id);
                        await SaveDisList(newList, oldList, origin, oldDc, newDc);
                        Trace.TraceInformation("Backup Collection End.  Name: '{0}'==>'{1}', Time: '{2}'", oldDc.Id,
                            newDc.Id,
                            DateTime.Now.ToString(CultureInfo.CurrentCulture));
                    }
                }
            }
        }


        private static async Task<bool> CheckSize(DocumentCollection oldDc, DocumentCollection origin)
        {
            double backRate;
            if (oldDc.Id == origin.Id)
            {
                //backRate = 0.5;
                backRate = (double) 5/100000;
            }
            else
            {
                backRate = (double) 5/100000;
            }

            var res = await _client.ReadDocumentCollectionAsync(oldDc.SelfLink);
            var size = res.CollectionSizeUsage;
            var totalSize = res.CollectionSizeQuota;

            return size > totalSize*backRate;
        }

        private static List<Hashtable> GenerateList(DocumentCollection oldDc)
        {
            var ds =
                from d in _client.CreateDocumentQuery<PostMessage>(oldDc.DocumentsLink)
                where d.Type == "Post"
                select d;

            Hashtable hs = new Hashtable();
            Hashtable newList = new Hashtable();
            Hashtable oldList = new Hashtable();

            var n = ds.ToList().Count;
            foreach (var d in ds)
            {
                if (!hs.ContainsKey(d.Path.District))
                {
                    hs.Add(d.Path.District, 1);
                }
                else
                {
                    hs[d.Path.District] = (int) hs[d.Path.District] + 1;
                }
            }

            foreach (DictionaryEntry h in hs)
            {
                var c = newList.Cast<DictionaryEntry>().Aggregate(0, (current, hh) => current + (int) hh.Value);

                if (c < n*0.45 && (c + (int) h.Value < n*0.55))
                {
                    newList.Add(h.Key, h.Value);
                }
                else
                {
                    oldList.Add(h.Key, h.Value);
                }
            }
            return new List<Hashtable> {oldList, newList};
        }

        private static async Task SaveDisList(Hashtable newList, Hashtable oldList,
            DocumentCollection origin, DocumentCollection oldDc,
            DocumentCollection newDc)
        {
            var allow = new DcAllocate
            {
                Type = "DisList",
                DcName = newDc.Id,
                DcSelfLink = newDc.SelfLink,
                District = new List<string>()
            };

            foreach (DictionaryEntry i in newList)
            {
                allow.District.Add(i.Key.ToString());
            }
            await _iDbService.ExecuteWithRetries(() => _client.CreateDocumentAsync(origin.DocumentsLink, allow));

            var ds =
                from d in _client.CreateDocumentQuery<DcAllocate>(origin.DocumentsLink)
                where d.Type == "DisList" && d.DcName == oldDc.Id
                select d;

            DcAllocate l = ds.AsEnumerable().FirstOrDefault();
            List<string> disList = (from DictionaryEntry i in oldList select i.Key.ToString()).ToList();

            if (l != null)
            {
                l.District = disList;
                await _iDbService.ExecuteWithRetries(() => _client.ReplaceDocumentAsync(l));
            }
            else
            {
                var allow2 = new DcAllocate
                {
                    Type = "DisList",
                    DcName = oldDc.Id,
                    DcSelfLink = oldDc.SelfLink,
                    District = disList
                };
                await _iDbService.ExecuteWithRetries(() => _client.CreateDocumentAsync(origin.DocumentsLink, allow2));
            }
        }

        private static async Task TransferDc(DocumentCollection oldDc, DocumentCollection newDc, Hashtable newList)
        {
            try
            {
                foreach (DictionaryEntry dis in newList)
                {
                    var tempDis = dis;
                    var items =
                        from d in _client.CreateDocumentQuery<PostMessage>(oldDc.DocumentsLink)
                        where d.Type == "Post" && d.Path.District == tempDis.Key.ToString()
                        select d;
                    foreach (var item in items)
                    {
                        var item1 = item;
                        var res =
                            await
                                _iDbService.ExecuteWithRetries(() => _client.CreateDocumentAsync(newDc.SelfLink, item1));
                        if (res.StatusCode == HttpStatusCode.Created)
                        {
                            //await _iDbService.DeleteDocument(_client, item1._self, 5);
                            var res2 =
                                await _iDbService.ExecuteWithRetries(() => _client.DeleteDocumentAsync(item1._self));
                            if (res2 == null)
                            {
                                Trace.TraceInformation("FFFFF");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error in processing " + e.Message);
            }
        }
    }
}