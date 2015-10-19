using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LMS.model.Models;
using LMS.service.Service;
using Microsoft.Azure;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace CheckRole
{
    public class LoadBalance
    {
        private static DocumentClient _client;
        private static Database _database;
        private static IDBService _iDbService;
        private static int _reTry = 5;
        private static DocumentCollection _curDc;

        public LoadBalance(string endpointUrl, string authorizationKey)
        {
            _client = new DocumentClient(new Uri(endpointUrl), authorizationKey);
            _database =
                _client.CreateDatabaseQuery().Where(db => db.Id == CloudConfigurationManager.GetSetting("Database"))
                    .AsEnumerable().FirstOrDefault();
            _iDbService = new DBService();
        }

        public async Task CheckBalance()
        {
            try
            {
                await _iDbService.OpenDB();
                _curDc = _iDbService.GetCurrentDc();
                if (_curDc != null)
                {
                    var run = await CheckSize(_curDc);
                    if (run)
                    {
                        var newDc = await GetNewCollection();

                        if (newDc != null)
                        {
                            try
                            {
                                await _iDbService.UpdateCurrentCollection(newDc);
                                await _iDbService.UpdateResolver(newDc);
                                await UpdatePerformanceLevel();
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
                    CheckBalance().Wait();
                }
                else
                {
                    Trace.TraceError("Error in start " + e.Message);
                }
            }
        }

        private static async Task UpdatePerformanceLevel()
        {
            var resolver = _iDbService.GetResolver();
            if (resolver.PartitionMap.Count == 2)
            {
                Offer offer = _client.CreateOfferQuery()
                    .Where(r => r.ResourceLink == resolver.PartitionMap.First().Value)
                    .AsEnumerable()
                    .SingleOrDefault();
                if (offer != null)
                {
                    offer.OfferType = "S2";
                    Offer updated = await _client.ReplaceOfferAsync(offer);
                }
                offer = _client.CreateOfferQuery()
                    .Where(r => r.ResourceLink == resolver.PartitionMap.Last().Value)
                    .AsEnumerable()
                    .SingleOrDefault();
                if (offer != null)
                {
                    offer.OfferType = "S3";
                    Offer updated = await _client.ReplaceOfferAsync(offer);
                }
            }
            else
            {
                var n = 1;
                var count = resolver.PartitionMap.Count;
                foreach (var d in resolver.PartitionMap)
                {
                    Offer offer = _client.CreateOfferQuery()
                        .Where(r => r.ResourceLink == d.Value)
                        .AsEnumerable()
                        .SingleOrDefault();
                    if (offer == null) continue;
                    if (n < count*0.5 && offer.OfferType != "S1")
                    {
                        offer.OfferType = "S1";
                        Offer updated = await _client.ReplaceOfferAsync(offer);
                    }
                    else if (n > 0.8*count && offer.OfferType != "S3")
                    {
                        offer.OfferType = "S3";
                        Offer updated = await _client.ReplaceOfferAsync(offer);
                    }
                    else if (offer.OfferType != "S2")
                    {
                        offer.OfferType = "S2";
                        Offer updated = await _client.ReplaceOfferAsync(offer);
                    }
                    n++;
                }
            }
        }

        private static async Task<DocumentCollection> GetNewCollection()
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
            DocumentCollection newDc = _client.CreateDocumentCollectionQuery(_database.SelfLink)
                .Where(c => c.Id == "LMSCollection1444075919174")
                .AsEnumerable()
                .FirstOrDefault();


            return newDc;
        }

        private static async Task<bool> CheckSize(DocumentCollection dc)
        {
            double backRate = (double) 2/100000;
            var res = await _client.ReadDocumentCollectionAsync(dc.SelfLink);
            var size = res.CollectionSizeUsage;
            var totalSize = res.CollectionSizeQuota;

            return size > totalSize*backRate;
        }
    }
}