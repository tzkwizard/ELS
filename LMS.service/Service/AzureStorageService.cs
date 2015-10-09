﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using LMS.model.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using StackExchange.Redis;

namespace LMS.service.Service
{
    public class AzureStorageService : IAzureStorageService
    {
        private static string storageConnectionString =
            "DefaultEndpointsProtocol=https;AccountName=elsaotuo;AccountKey=AV49N0PZ1Qlz42b0w47EPoPbNLULgxYOWxsO4IvFmrAkZPzkdGCKKOJqyiHVGfAPex6HhkDSWpNQAIuPmBHBMA==";

        private CloudTable _cloudTable;

        public AzureStorageService()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient c = storageAccount.CreateCloudTableClient();
            _cloudTable = c.GetTableReference("Chat");
            _cloudTable.CreateIfNotExists();
        }

        public LMSChatresult SearchChat(string roomId, long start)
        {
            var t = (long) (DateTime.UtcNow.AddMonths(-1).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            List<TableChat> items = new List<TableChat>();
            var t1 = DateTime.Now;
            var n = 1;
            var i = 1;
            long end=start;
            while (items.Count < 5 && end > t)
            {
                i = i + n;
                end = (long)(DateTime.UtcNow.AddHours(-6 * i).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
                TableQuery<TableChat> rangeQuery = new TableQuery<TableChat>().Where(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition("PartitionKey", "eq", roomId),
                        TableOperators.And,
                        TableQuery.CombineFilters(
                            TableQuery.GenerateFilterConditionForLong("timestamp", QueryComparisons.LessThan,
                                start),
                            TableOperators.And,
                            TableQuery.GenerateFilterConditionForLong("timestamp", QueryComparisons.GreaterThan,
                                end)
                            )
                        )
                    );
                items = _cloudTable.ExecuteQuery(rangeQuery).OrderBy(o => o.timestamp).ToList();
                n++;
            }
            var res = new LMSChatresult
            {
                moreData = false,
                time = end,
                list = items
            };

            return res;
        }
    }
}