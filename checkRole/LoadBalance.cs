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
        private static int reTry = 5;
        private static DocumentCollection _curDc;
        private static DocumentCollection _originDc;
        private static CurrentCollection _curCol;

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
                _curDc = GetCurrentDc();
                if (_curDc != null)
                {
                    var run = await CheckSize(_curDc);
                    if (run)
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
                            try
                            {
                                await UpdateCurrentCollection(newDc);
                                await _iDbService.UpdateResolver(_client, _originDc, newDc);
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
                if (reTry > 0)
                {
                    reTry--;
                    Thread.Sleep(60000);
                    Trace.TraceInformation("Restart CheckBalance... in 1 min " + reTry);
                    CheckBalance().Wait();
                }
                else
                {
                    Trace.TraceError("Error in start " + e.Message);
                }
            }
        }

        private static async Task UpdateCurrentCollection(DocumentCollection newDc)
        {
            await _iDbService.ExecuteWithRetries(() => _client.DeleteDocumentAsync(_curCol._self));
            await _client.CreateDocumentAsync(_originDc.SelfLink, new CurrentCollection
            {
                id = "CurrentCollection",
                name = newDc.Id
            });
        }

        private static DocumentCollection GetCurrentDc()
        {
            _originDc = _client.CreateDocumentCollectionQuery(_database.SelfLink)
                .Where(c => c.Id == CloudConfigurationManager.GetSetting("MasterCollection"))
                .AsEnumerable()
                .FirstOrDefault();

            if (_originDc != null)
            {
                _curCol =
                    _client.CreateDocumentQuery<CurrentCollection>(_originDc.SelfLink)
                        .Where(x => x.id == "CurrentCollection")
                        .AsEnumerable()
                        .FirstOrDefault();
                if (_curCol != null)
                {
                    return _client.CreateDocumentCollectionQuery(_database.SelfLink)
                        .Where(c => c.Id == _curCol.name)
                        .AsEnumerable()
                        .FirstOrDefault();
                }
            }
            return null;
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