using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using LMS.Common.Models;
using LMS.Common.Models.Api;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace LMS.Common.DAL
{
    public class ASoperation : IASoperation
    {
        private static string _storageConnectionString;

        private static CloudTableClient _cloudTableClient;

        public ASoperation()
        {
            _storageConnectionString = ConfigurationManager.AppSettings["AzureWebJobsStorage"] ??
                                       CloudConfigurationManager.GetSetting("AzureWebJobsStorage");
        }

        public CloudTableClient GetTableClient()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_storageConnectionString);
            _cloudTableClient = _cloudTableClient ?? storageAccount.CreateCloudTableClient();
            return _cloudTableClient;
        }

        public List<TableChat> GetChat(string roomId, long start, long end)
        {
            var client = GetTableClient();
            var cloudTable = client.GetTableReference("Chat");
            cloudTable.CreateIfNotExists();

            TableQuery<TableChat> rangeQuery = new TableQuery<TableChat>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", "eq", roomId),
                    TableOperators.And,
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterConditionForLong("timestamp", QueryComparisons.LessThan,
                            end),
                        TableOperators.And,
                        TableQuery.GenerateFilterConditionForLong("timestamp", QueryComparisons.GreaterThan,
                            start)
                        )
                    )
                );
            return cloudTable.ExecuteQuery(rangeQuery).OrderBy(o => o.timestamp).ToList();
        }
    }
}