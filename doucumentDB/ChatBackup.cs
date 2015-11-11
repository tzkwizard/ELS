using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FireSharp.Interfaces;
using FireSharp.Response;
using LMS.Common.Models;
using LMS.Common.Service;
using LMS.Common.Service.Interface;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Microsoft.Practices.TransientFaultHandling;
using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.AzureStorage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Incremental = Microsoft.Practices.TransientFaultHandling.Incremental;


namespace doucumentDB
{
    class ChatBackup
    {
        private static string storageConnectionString =
          "DefaultEndpointsProtocol=https;AccountName=elsaotuo;AccountKey=AV49N0PZ1Qlz42b0w47EPoPbNLULgxYOWxsO4IvFmrAkZPzkdGCKKOJqyiHVGfAPex6HhkDSWpNQAIuPmBHBMA==";
        private static IFirebaseClient _firebaseClient;

        private const int RecordRemained = 2;

        public static void TableDemo()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient c = storageAccount.CreateCloudTableClient();
            CloudTable table = c.GetTableReference("Chat");
            table.CreateIfNotExists();

            IDbService i = new DbService();
            _firebaseClient = i.GetFirebaseClient();
            //Search3(table);
            //Search2(table);
            //Search3(table);
            Insert(table).Wait();
            //BackupDocumentChat(table).Wait();

           
        }

        private static void Search(CloudTable table)
        {
            TableQuery<TableChat> query =
                new TableQuery<TableChat>().Where(TableQuery.GenerateFilterCondition("user",
                    QueryComparisons.Equal, "Tzkwizard"));

            // Print the fields for each customer.
            foreach (TableChat entity in table.ExecuteQuery(query))
            {
                Console.WriteLine("{0}, {1} {2} {3} {4}", entity.PartitionKey, entity.RowKey,
                    entity.roomName, entity.uid, entity.message);
            }
        }


        private static void Search2(CloudTable table)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<CustomerEntity>("Harp", "Ben");

            // Execute the retrieve operation.
            TableResult retrievedResult = table.Execute(retrieveOperation);

            // Print the phone number of the result.
            if (retrievedResult.Result != null)
                Console.WriteLine(((CustomerEntity)retrievedResult.Result).PhoneNumber);
            else
                Console.WriteLine("The phone number could not be retrieved.");
        }


        private static void Search3(CloudTable table)
        {
            TableQuery<TableChat> rangeQuery = new TableQuery<TableChat>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("uid", "eq", "1201818"),
                    TableOperators.And,
                    TableQuery.GenerateFilterConditionForLong("timestamp", QueryComparisons.GreaterThan, 1444414477058)
                    ));


            // Loop through the results, displaying information about the entity.
            foreach (TableChat entity in table.ExecuteQuery(rangeQuery))
            {
                Console.WriteLine("{0}, {1} {2} {3} {4}", entity.PartitionKey, entity.RowKey,
                    entity.roomName, entity.user,entity.message);
            }
        }

        public static async Task Insert(CloudTable table)
        {
            CustomerEntity customer1 = new CustomerEntity("Harp", "Walters")
            {
                Email = "Walter@contoso.com",
                PhoneNumber = "425-555-0101"
            };
            CustomerEntity customer2 = new CustomerEntity("Harp", "Ben")
            {
                Email = "Ben@contoso.com",
                PhoneNumber = "425-555-0102"
            };
            TableBatchOperation batchOperation = new TableBatchOperation();
            // Create the TableOperation object that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(customer1);
            batchOperation.Insert(customer1);
            batchOperation.Insert(customer2);
            // Execute the insert operation.

            Incremental retryStrategy = new Incremental(5, TimeSpan.FromSeconds(1),
   TimeSpan.FromSeconds(2));
            ExponentialBackoff retryStrategy2=new ExponentialBackoff(5,TimeSpan.FromSeconds(1),TimeSpan.FromSeconds(5),TimeSpan.FromSeconds(2));
            TimeSpan back = TimeSpan.FromSeconds(31);
 
            // Define your retry policy using the retry strategy and the Azure storage
            // transient fault detection strategy.
            RetryPolicy<StorageTransientErrorDetectionStrategy> retryPolicy =
              new RetryPolicy<StorageTransientErrorDetectionStrategy>(retryStrategy);

            RetryPolicy<StorageTransientErrorDetectionStrategy> r =
                new RetryPolicy<StorageTransientErrorDetectionStrategy>(retryStrategy2);
            // Receive notifications about retries.
            retryPolicy.Retrying += (sender, args) =>
            {
                Console.WriteLine("Information");
                // Log details of the retry.
                var msg = String.Format("Retry - Count:{0}, Delay:{1}, Exception:{2}",
                    args.CurrentRetryCount, args.Delay, args.LastException);
                Console.WriteLine(msg, "Information");
            };

            try
            {
                // Do some work that may result in a transient fault.
              await retryPolicy.ExecuteAsync(
                  () => table.ExecuteBatchAsync(batchOperation));
            }
            catch (Exception e)
            {
                var z = e;
            }
            Console.ReadLine();












          /*  // Create a new customer entity.
            CustomerEntity customer1 = new CustomerEntity("Harp", "Walters")
            {
                Email = "Walter@contoso.com",
                PhoneNumber = "425-555-0101"
            };
            CustomerEntity customer2 = new CustomerEntity("Harp", "Ben")
            {
                Email = "Ben@contoso.com",
                PhoneNumber = "425-555-0102"
            };
            TableBatchOperation batchOperation = new TableBatchOperation();
            // Create the TableOperation object that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(customer1);
            batchOperation.Insert(customer1);
            batchOperation.Insert(customer2);
            // Execute the insert operation.
            try
            {
                IList<TableResult> z = await table.ExecuteBatchAsync(batchOperation);
                foreach (var i in z)
                {
                    CustomerEntity y = (CustomerEntity)i.Result;
                    Console.WriteLine(y.PartitionKey);
                }
                var x = z;
            }
            catch (StorageException e)
            {

                var z = e.RequestInformation.HttpStatusCode;
                var zz = 3;
            }*/
            
        }
        private static async Task BackupDocumentChat(CloudTable table)
        {
            FirebaseResponse chatResponse = _firebaseClient.Get("ChatRoom");
            dynamic ds = JsonConvert.DeserializeObject(chatResponse.Body);

            foreach (var d in ds)
            {
                dynamic t = d;
                await Backup(t, table);
            }
        }

        private static async Task Backup(dynamic u, CloudTable table)
        {
            try
            {
                if (u.Value.messages.Value != "")
                {
                    var origin = new List<dynamic>();
                    foreach (var s in u.Value.messages)
                    {
                        origin.Add(s);
                    }

                    var sorted = origin.OrderBy(o => o.Value.timestamp).ToList();

                    if (sorted.Count <= RecordRemained) return;

                    for (int i = 0; i < sorted.Count - RecordRemained; i++)
                    {
                        var s = sorted[i];
                        TableChat c = new TableChat(u.Value.room.ID.ToString(), s.Name.ToString())
                        {
                            roomName = u.Value.room.Name,
                            timestamp = s.Value.timestamp,
                            user = s.Value.user,
                            uid = s.Value.uid,
                            message = s.Value.message
                        };
                        TableOperation insertOperation = TableOperation.Insert(c);
                        // Execute the insert operation.
                        var res = await table.ExecuteAsync(insertOperation);
                        if (res.HttpStatusCode == 204)
                        {
                            string url = "ChatRoom/" + u.Name + "/messages/" + s.Name;
                            await _firebaseClient.DeleteAsync(url);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
