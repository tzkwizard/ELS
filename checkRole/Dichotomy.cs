using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LMS.model.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.WindowsAzure;
using Microsoft.Azure;

namespace CheckRole
{
    public class Dichotomy
    {
        private static DocumentClient _client;
        private static Database _database;
        private static int reTry = 5;

        public Dichotomy(string endpointUrl, string authorizationKey)
        {
            _client = new DocumentClient(new Uri(endpointUrl), authorizationKey);
            _database =
                _client.CreateDatabaseQuery().Where(db => db.Id == CloudConfigurationManager.GetSetting("Database"))
                    .AsEnumerable().FirstOrDefault();
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
                        Console.WriteLine(x.Id + x.DocumentsLink);
                        await UpdateDc(x, _client, _database, origin);
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Error in updateDc " + e.Message);
                if (reTry > 0)
                {
                    Trace.TraceInformation("Retry...  " + reTry);
                    UpdateDcAll().Wait();
                }
                else
                {
                    Trace.TraceError("Run out of retry");
                }
                reTry--;
            }
        }

        private static async Task<bool> CheckSize(DocumentCollection oldDc, DocumentClient client,
            DocumentCollection origin)
        {
            double backRate;
            if (oldDc.Id == origin.Id)
            {
                //backRate = 0.5;
                backRate = (double) 2/100000;
            }
            else
            {
                backRate = (double) 2/100000;
            }

            var res = await client.ReadDocumentCollectionAsync(oldDc.SelfLink);
            var size = res.CollectionSizeUsage;
            var totalSize = res.CollectionSizeQuota;

            return size > totalSize*backRate;
        }

        private static async Task UpdateDc(DocumentCollection oldDc, DocumentClient client,
            Database database, DocumentCollection origin)
        {
            var run = await CheckSize(oldDc, client, origin);

            if (run)
            {
                Trace.TraceInformation("Backup Collection Start.  Name: '{0}', Time: '{1}'", oldDc.Id,
                    DateTime.Now.ToString(CultureInfo.CurrentCulture));
                var ds =
                    from d in client.CreateDocumentQuery<PostMessage>(oldDc.DocumentsLink)
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
                    DocumentCollection newDc = client.CreateDocumentCollectionQuery(database.SelfLink)
                        .Where(c => c.Id == "LMSCollection1444075919174")
                        .AsEnumerable()
                        .FirstOrDefault();

                    TransferDc(oldDc, newDc, client, newList).Wait();
                    await SaveDisList(newList, oldList, origin, oldDc, newDc, client);
                }
            }
        }

        private static async Task SaveDisList(Hashtable newList, Hashtable oldList,
            DocumentCollection origin, DocumentCollection oldDc,
            DocumentCollection newDc, DocumentClient client)
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
            await client.CreateDocumentAsync(origin.DocumentsLink, allow);


            var ds =
                from d in client.CreateDocumentQuery<DcAllocate>(origin.DocumentsLink)
                where d.Type == "DisList" && d.DcName == oldDc.Id
                select d;

            var l = ds.ToList().FirstOrDefault();

            if (l != null) await client.DeleteDocumentAsync(l._self);


            var allow2 = new DcAllocate
            {
                Type = "DisList",
                DcName = oldDc.Id,
                DcSelfLink = oldDc.SelfLink,
                District = new List<string>(),
            };

            foreach (DictionaryEntry i in oldList)
            {
                allow2.District.Add(i.Key.ToString());
            }
            await client.CreateDocumentAsync(origin.DocumentsLink, allow2);
        }

        private static async Task TransferDc(DocumentCollection oldDc, DocumentCollection newDc, DocumentClient client,
            Hashtable newList)
        {
            foreach (DictionaryEntry dis in newList)
            {
                var tempDis = dis;
                var items =
                    from d in client.CreateDocumentQuery<PostMessage>(oldDc.DocumentsLink)
                    where d.Type == "Post" && d.Path.District == tempDis.Key.ToString()
                    select d;
                foreach (var item in items)
                {
                    await client.DeleteDocumentAsync(item._self);
                    await client.CreateDocumentAsync(newDc.DocumentsLink, item);
                }
            }
        }
    }
}