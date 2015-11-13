using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LMS.Common.Service;
using LMS.Common.Service.Interface;
using Microsoft.Azure;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace CheckRole
{
    public class LoadBalance
    {
        private static IDbService _iDbService;
        private static int _reTry = 5;

        public LoadBalance()
        {
            _iDbService = _iDbService ?? new DbService();
        }

        public async Task CheckBalance(string databaseSelfLink)
        {
            try
            {
                var client = _iDbService.GetDocumentClient();
                await client.OpenAsync();
                var curDc = _iDbService.DBoperation().GetCurrentDc();
                if (curDc != null)
                {
                    var run = await CheckSize(curDc, client);
                    if (run)
                    {
                        var newDc = await GetNewCollection(client, databaseSelfLink);

                        if (newDc != null)
                        {
                            try
                            {
                                await _iDbService.CollectionService().UpdateCurrentCollection(newDc);
                                await _iDbService.RangePartitionResolver().UpdateResolver(newDc);
                                await  UpdatePerformanceLevel(client);
                            }
                            catch (Exception e)
                            {
                                Trace.TraceError("Error in update CurrentCollection and Resolver");
                            }
                        }
                    }
                }
                else
                {
                    Trace.TraceError("Not Find Current Collection");
                }
            }
            catch (Exception e)
            {
                if (_reTry > 0)
                {
                    _reTry--;
                    Thread.Sleep(60000);
                    Trace.TraceInformation("Restart CheckBalance... in 1 min " + _reTry);
                    CheckBalance(databaseSelfLink).Wait();
                }
                else
                {
                    Trace.TraceError("Error in start " + e.Message);
                }
            }
        }

        private static async Task UpdatePerformanceLevel(DocumentClient client)
        {
            var resolver = _iDbService.RangePartitionResolver().GetResolver();
            if (resolver.PartitionMap.Count == 2)
            {
                Offer offer = client.CreateOfferQuery()
                    .Where(r => r.ResourceLink == resolver.PartitionMap.First().Value)
                    .AsEnumerable()
                    .SingleOrDefault();
                if (offer != null)
                {
                    offer.OfferType = "S2";
                    Offer updated = await client.ReplaceOfferAsync(offer);
                }
                offer = client.CreateOfferQuery()
                    .Where(r => r.ResourceLink == resolver.PartitionMap.Last().Value)
                    .AsEnumerable()
                    .SingleOrDefault();
                if (offer != null)
                {
                    offer.OfferType = "S3";
                    Offer updated = await client.ReplaceOfferAsync(offer);
                }
            }
            else
            {
                var n = 1;
                var count = resolver.PartitionMap.Count;
                foreach (var d in resolver.PartitionMap)
                {
                    Offer offer = client.CreateOfferQuery()
                        .Where(r => r.ResourceLink == d.Value)
                        .AsEnumerable()
                        .SingleOrDefault();
                    if (offer == null) continue;
                    if (n < count*0.5 && offer.OfferType != "S1")
                    {
                        offer.OfferType = "S1";
                        Offer updated = await client.ReplaceOfferAsync(offer);
                    }
                    else if (n > 0.8*count && offer.OfferType != "S3")
                    {
                        offer.OfferType = "S3";
                        Offer updated = await client.ReplaceOfferAsync(offer);
                    }
                    else if (offer.OfferType != "S2")
                    {
                        offer.OfferType = "S2";
                        Offer updated = await client.ReplaceOfferAsync(offer);
                    }
                    n++;
                }
            }
        }

        private static async Task<DocumentCollection> GetNewCollection(DocumentClient client, string databaseSelfLink)
        {
            //create new collection
            /*var t = (long) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            DocumentCollection newDc=await _client.CreateDocumentCollectionAsync(_database.CollectionsLink,
                                            new DocumentCollection
                                            {
                                                Id = "LMSCollection"+t
                                            });
            //Fetch the resource to be updated
            Offer offer = _client.CreateOfferQuery()
                                      .Where(r => r.ResourceLink == newDc.SelfLink)
                                      .AsEnumerable()
                                      .SingleOrDefault();

            //Change the user mode to All
            if (offer != null)
            {
                offer.OfferType = "S3";

                //Now persist these changes to the database by replacing the original resource
                Offer updated = await _client.ReplaceOfferAsync(offer);
            }*/

            //search collection
            DocumentCollection newDc = client.CreateDocumentCollectionQuery(databaseSelfLink)
                .Where(c => c.Id == "LMSCollection1444075919174")
                .AsEnumerable()
                .FirstOrDefault();


            return newDc;
        }

        private static async Task<bool> CheckSize(DocumentCollection dc, DocumentClient client)
        {
            double backRate = (double) 2/100000;
            var res = await client.ReadDocumentCollectionAsync(dc.SelfLink);
            var size = res.CollectionSizeUsage;
            var totalSize = res.CollectionSizeQuota;

            return size > totalSize*backRate;
        }
    }
}