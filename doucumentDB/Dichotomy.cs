using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessageHandleApi.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace doucumentDB
{
    public class Dichotomy
    {
        public static async Task UpdateDcAll(string endpointUrl, string authorizationKey)
        {
            DocumentClient client = new DocumentClient(new Uri(endpointUrl), authorizationKey);
            var database = await Program.GetDB(client);

            IEnumerable<DocumentCollection> dz = client.CreateDocumentCollectionQuery(database.SelfLink)
                .AsEnumerable();


            DocumentCollection origin = client.CreateDocumentCollectionQuery(database.SelfLink)
                .Where(c => c.Id == "LMSCollection")
                .AsEnumerable()
                .FirstOrDefault();

            //search collection
            var ds =
                from d in client.CreateDocumentQuery<DcAllocate>(origin.DocumentsLink)
                where d.Type == "DisList"
                select d;
            foreach (var d in ds)
            {
                if (d.District.Contains("tst-azhang14"))
                {
                    Console.WriteLine(d.DcName);
                }
            }

            /* foreach (var x in dz)
            {
                Console.WriteLine(x.Id + x.DocumentsLink);
                await UpdateDc(x, client, database, origin);
            }*/
        }

        private static async Task UpdateDc(DocumentCollection oldDc, DocumentClient client,
            Database database, DocumentCollection origin)
        {
            var res = await client.ReadDocumentCollectionAsync(oldDc.SelfLink);
            var size = res.CollectionSizeUsage;
            var totalSize = res.CollectionSizeQuota;
            if (size > totalSize*2/100000)
            {
                var ds =
                    from d in client.CreateDocumentQuery<PostMessage>(oldDc.DocumentsLink)
                    where d.Type == "Post"
                    select d;

                Hashtable hs = new Hashtable();
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
                Hashtable newList = new Hashtable();
                Hashtable oldList = new Hashtable();

                foreach (DictionaryEntry h in hs)
                {
                    var c = 0;
                    foreach (DictionaryEntry hh in newList)
                    {
                        c = c + (int) hh.Value;
                    }
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

                    await SaveDisList(newList, oldList, origin, oldDc, newDc, client);

                    await TransferDc(oldDc, newDc, client, database, newList);
                }
            }
        }

        private static async Task SaveDisList(Hashtable newList, Hashtable oldList,
            DocumentCollection origin, DocumentCollection oldDc,
            DocumentCollection newDc, DocumentClient client)
        {
            var t = (long) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
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
            Database database, Hashtable newList)
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
                    try
                    {
                        await client.DeleteDocumentAsync(item._self);
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(200);
                        client.DeleteDocumentAsync(item._self).Wait();
                    }
                    try
                    {
                        await client.CreateDocumentAsync(newDc.DocumentsLink, item);
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(200);
                        client.CreateDocumentAsync(newDc.DocumentsLink, item).Wait();
                    }
                }
            }
        }
    }
}