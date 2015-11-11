using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LMS.Common.Models;
using LMS.Common.Service;
using LMS.Common.Service.Interface;
using Microsoft.Azure;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents.Partitioning;
using Microsoft.Practices.TransientFaultHandling;
using Newtonsoft.Json;
using Database = Microsoft.Azure.Documents.Database;


namespace doucumentDB
{
    internal class DocumentDB
    {
        private static string EndpointUrl = "https://tstaz.documents.azure.com:443/";

        private static string AuthorizationKey =
            "6xPkxpC7FyiozobQOtQ8yFxbqd7uLOCz0pRo4i+GKxHdmISxDrMKZdaKQH0/0BJe/xC3UKdQM4C1x5d4Rxk3AQ==";

        private static string EndpointUrl2 = "https://azurelmsdb.documents.azure.com:443/";

        private static string AuthorizationKey2 =
            "mlc6CIPF4GKaz7KekD+fAjcB/k6AkUh7chjKham+WuM2qHFrAg1Untll5g/x5uBEJTu6fBZ+7bifvpWZl1Xj6g==";

        private static IDbService _iDbService;

        public static async Task GetStartedDemo()
        {
            // Create a new instance of the DocumentClient.
            DocumentClient client = new DocumentClient(new Uri(EndpointUrl2), AuthorizationKey2);
            ConfigurationManager.AppSettings["DocumentDBUrl"] = EndpointUrl2;
            ConfigurationManager.AppSettings["DocumentDBAuthorizationKey"] = AuthorizationKey2;
            var database = await GetDB(client, "LMS");
            var documentCollection = await GetDC(client, database);
            _iDbService = new DbService();




            // await DeleteAll(client, database, documentCollection);
            await sp2(documentCollection, client, database);
            //await GetData(client, documentCollection);
            //ReadData(client, documentCollection);
            //await WriteData(client, documentCollection);
            Console.ReadLine();
            /*await client.DeleteDatabaseAsync(database.SelfLink);
             client.Dispose();*/
        }

        private static async Task sp2(DocumentCollection dc, DocumentClient client, Database database)
        {

            //transfer collection data
             DocumentCollection dc2 = client.CreateDocumentCollectionQuery(database.SelfLink)
                .Where(c => c.Id == "LMSCollection1444075919174")
                .AsEnumerable()
                .FirstOrDefault();
            await client.OpenAsync();
            //await _iDbService.CollectionTransfer(client, dc2, dc);
            var mn=await client.ReadDatabaseAsync(database.SelfLink);




            /*  await client.CreateDocumentAsync(dc.SelfLink, new CurrentCollection
            {
                id = "CurrentCollection",
                name=dc.Id
            });*/
            //var resolver = _iDbService.GetResolver(client);

            /* HashPartitionResolver hashResolver = new HashPartitionResolver(
                u => ((PostMessage) u).Path.District,
                new string[] {dc.SelfLink, dc2.SelfLink});

            client.PartitionResolvers[database.SelfLink] = hashResolver;*/

            /* var rangeResolver = _iDbService.GetResolver(client);
            client.PartitionResolvers[database.SelfLink] = rangeResolver;*/


            var created = await _iDbService.RangePartitionResolver().InitResolver();


             while (true)
            {
                var re2 = await _iDbService.RangePartitionResolver().UpdateResolver(dc2);
                var p = re2;
                await Task.Delay(TimeSpan.FromSeconds(4));
            }

            /*  var z1 = rangeResolver.GetPartitionKey(new PostMessage
                    {
                        Type = "Post",
                        Info = new Info
                        {
                            user = "tzk",
                            uid = "1210808",
                            message = "java",
                            timestamp = 7
                        },
                        Path = new PostPath
                        {
                            District = "tst-azhang" 
                        }
                    });*/

            //search global
            IQueryable<PostMessage> query = client.CreateDocumentQuery<PostMessage>(database.SelfLink)
                .Where(u => u.Info.timestamp > 1);

           
            //search on partition
            var partitionKey = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            var query2 = client.CreateDocumentQuery<PostMessage>(database.SelfLink, new FeedOptions
            {
                MaxItemCount = 1200
            }, partitionKey);
           
            foreach (PostMessage a in query2)
            {
                Console.WriteLine(a.Info.timestamp);
            }


            //check search RU
            /*  double totalRequestCharge = 0;
            var query3 = client.CreateDocumentQuery<PostMessage>(dc2.SelfLink).AsDocumentQuery();
            while (query3.HasMoreResults)
            {
                FeedResponse<dynamic> queryResponse = await query3.ExecuteNextAsync<dynamic>();
                Console.WriteLine("Query batch consumed {0} request units {1} doc", queryResponse.RequestCharge, queryResponse.Count);
                totalRequestCharge += queryResponse.RequestCharge;
            }
            Console.WriteLine("Query consumed {0} request units in total", totalRequestCharge);*/
    
            //await AddTestData(client,database,5);

            Console.ReadLine();
        }

        private static async Task AddTestData(DocumentClient client, Database database, int number)
        {
            int n = 0;
            try
            {
                while (n < number)
                {
                    n++;
                    var timestamp =
                        (long) (DateTime.UtcNow.AddHours(-1).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
                    PostMessage x = new PostMessage
                    {
                        Type = "Post",
                        Info = new Info
                        {
                            user = "tzk",
                            uid = "1210808",
                            message = "java",
                            timestamp = timestamp
                        },
                        Path = new PostPath
                        {
                            District = "tst-azhang"
                        }
                    };

                    await client.CreateDocumentAsync(database.SelfLink, x);
                }
            }
            catch (Exception e)
            {
                var t = e;
            }
        }

        public static async Task<Database> GetDB(DocumentClient client, string name)
        {
            Database database = client.CreateDatabaseQuery().Where(db => db.Id == name)
                .AsEnumerable().FirstOrDefault();
            /*?? await client.CreateDatabaseAsync(
                                    new Database
                                    {
                                        Id = "LMSRegistry"
                                    });*/

            Console.WriteLine(database.SelfLink);
            return database;
        }

        public static async Task<DocumentCollection> GetDC(DocumentClient client, Database database)
        {
            DocumentCollection documentCollection = client.CreateDocumentCollectionQuery(database.SelfLink)
                .Where(c => c.Id == "LMSCollection")
                .AsEnumerable()
                .FirstOrDefault()
                                                    ??
                                                    await client.CreateDocumentCollectionAsync(database.CollectionsLink,
                                                        new DocumentCollection
                                                        {
                                                            Id = "LMSCollection"
                                                        });

            Console.WriteLine(documentCollection.SelfLink);
            return documentCollection;
        }


        private static async Task DeleteAll(DocumentClient client, Database database,
            DocumentCollection dc)
        {
            IEnumerable<DocumentCollection> dz = client.CreateDocumentCollectionQuery(database.SelfLink)
                .AsEnumerable();
            var nnn = 0;
            foreach (var z in dz)
            {
                var families =
                    from f in client.CreateDocumentQuery(z.SelfLink)
                    select f;

                try
                {
                    var mn = 0;
                    List<dynamic> batch = new List<dynamic>();

                    foreach (var family in families)
                    {
                        nnn++;

                        if (mn < 100)
                        {
                            batch.Add(family);
                            mn++;
                        }
                        else
                        {
                            await _iDbService.DBoperation().BatchDelete(z, batch);
                            mn = 0;
                            batch = new List<dynamic>();
                        }

                        /* dynamic d = JsonConvert.DeserializeObject(family.ToString());
                            //Console.WriteLine(d.Path.school);  
                            try
                            {
                                //var res = await client.DeleteDocumentAsync(family.SelfLink);
                                var family1 = family;
                                var response =
                                    await
                                        _iDbService.ExecuteWithRetries(5,
                                            () => client.DeleteDocumentAsync(family1.SelfLink));
                            }
                            catch (DocumentClientException e)
                            {
                                Console.WriteLine(e.Message);
                            }

                            Console.WriteLine(family.SelfLink);*/
                    }
                    await _iDbService.DBoperation().BatchDelete(z, batch);
                }
                catch (Exception eee)
                {
                    Console.WriteLine(eee.Message);
                }
            }

            Console.WriteLine(nnn);
        }

        private static void sp()
        {
            var markAntiquesSproc = new StoredProcedure
            {
                Id = "ValidateDocumentAge",
                Body = @"
                function(docToCreate, antiqueYear) {
                   var collection = getContext().getCollection();    
                   var response = getContext().getResponse();    

                   if(docToCreate.Year != undefined && docToCreate.Year < antiqueYear){
                    docToCreate.antique = true;
                   }

                   collection.createDocument(collection.getSelfLink(), docToCreate, {}, 
                    function(err, docCreated, options) { 
                        if(err) throw new Error('Error while creating document: ' + err.message);                              
                        if(options.maxCollectionSizeInMb == 0) throw 'max collection size not found'; 
                        response.setBody(docCreated);
                    });
                 }"
            };

            // register stored procedure
            /* var res =
                 client.CreateStoredProcedureQuery(documentCollection.SelfLink).Where(c => c.Id == "helloWorld")
                     .AsEnumerable()
                     .FirstOrDefault();

             if (res != null)
             {
                 await client.DeleteStoredProcedureAsync(res.SelfLink);
             }
             StoredProcedure createdStoredProcedure =
                 await client.CreateStoredProcedureAsync(documentCollection.SelfLink, markAntiquesSproc);*/

            // execute stored procedure
            /*var createdDocument =
                await client.ExecuteStoredProcedureAsync<string>(res.SelfLink);*/

            for (int i = 0; i < 5; i++)
            {
                var n = new Random();
                dynamic document = new Document() {Id = "-JzLyG5lptD9dWAfMOWt" + i};
                document.uid = 1201818;
                document.user = "Tzkwizard";
                document.message = "Java";
                document.timestamp = 1442420691385;
                // await client.ExecuteStoredProcedureAsync<Document>("dbs/g5w9AA==/colls/g5w9AP1fOwA=/sprocs/g5w9AP1fOwAVAAAAAAAAgA==/", document, 1920);
            }
        }

        private static void ReadData(DocumentClient client, DocumentCollection documentCollection)
        {
            /*// Query the documents using LINQ.
            var families =
                from f in client.CreateDocumentQuery(documentCollection.DocumentsLink)
                where f.Id == "AndersenFamily"
                select f;

            foreach (var family in families)
            {
                Console.WriteLine("\tRead {0} from LINQ", family);
            }

            families = client.CreateDocumentQuery(documentCollection.DocumentsLink)
                .Where(f => f.Id == "AndersenFamily")
                .Select(f => f);

            foreach (var family in families)
            {
                Console.WriteLine("\tRead {0} from LINQ query", family);
            }
*/

            // Query the documents using DocumentSQL with one join.
            var items = client.CreateDocumentQuery<dynamic>(documentCollection.DocumentsLink,
                "SELECT f.id, c.FirstName AS child " +
                "FROM Families f " +
                "JOIN c IN f.Children");

            foreach (var item in items.ToList())
            {
                Console.WriteLine(item);
            }

            // Query the documents using LINQ with one join.
            /*var items = client.CreateDocumentQuery<Family>(documentCollection.DocumentsLink)
                .SelectMany(family => family.Parents
                    .Select(person => new
                    {
                        family = family.Id,
                        parent = person.FirstName
                    })
                );

            foreach (var item in items.ToList())
            {
                Console.WriteLine(item);
            }


            foreach (var pet in
                client.CreateDocumentQuery<Family>(documentCollection.DocumentsLink)
                    .SelectMany(f => f.Children)
                    .SelectMany(c => c.Pets
                        .Select(p => new
                        {
                            pet = p.GivenName
                        })))

            {
                Console.WriteLine(pet);
            }*/
        }


        private static async Task WriteData(DocumentClient client, DocumentCollection documentCollection)
        {
            var z = new Random();
            var topic = new Topic
            {
                Id = "123",
                Type = "Post",
                Path = new PostPath
                {
                    District = "tst-azhang1",
                    School = "JP",
                    Classes = "Math",
                    Unit = "Calculus"
                },
                Info = new InfoPost
                {
                    teacher = "Newton",
                    start = "2015-4-1",
                    due = DateTime.Now.AddDays(z.Next(1, 4)).ToString(CultureInfo.CurrentCulture),
                    description = "homework"
                }
            };
            //var res = await client.CreateDocumentAsync(documentCollection.DocumentsLink, topic);
            await
                RetryService.ExecuteWithRetries(5,
                    () => client.CreateDocumentAsync(documentCollection.DocumentsLink, topic));
            var i = 3;


            /*    var families =
                from f in client.CreateDocumentQuery<Topic>(documentCollection.DocumentsLink)
                where f.Type == "Topic"
                select f;
            foreach (var family in families)
            {              
                var s = JsonConvert.SerializeObject(family);
                dynamic d = JsonConvert.DeserializeObject(s);
                var res = await client.DeleteDocumentAsync(family._self);
                Console.Write(d);             
            }*/
        }


        private static async Task GetData(DocumentClient client, DocumentCollection documentCollection)
        {
            var t = (long) (DateTime.UtcNow.AddHours(-4).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;

            // Create the Andersen family document.
            Family AndersenFamily = new Family
            {
                Id = "AndersenFamily",
                LastName = "Andersen",
                Parents = new Parent[]
                {
                    new Parent {FirstName = "Thomas"},
                    new Parent {FirstName = "Mary Kay"}
                },
                Children = new Child[]
                {
                    new Child
                    {
                        FirstName = "Henriette Thaulow",
                        Gender = "female",
                        Grade = 5,
                        Pets = new Pet[]
                        {
                            new Pet {GivenName = "Fluffy"}
                        }
                    }
                },
                Address = new Address {State = "WA", County = "King", City = "Seattle"},
                IsRegistered = true
            };
            try
            {
                var res = await client.CreateDocumentAsync(documentCollection.DocumentsLink, AndersenFamily);
            }
            catch (DocumentClientException e)
            {
                var z = e;
            }
        }
    }
}