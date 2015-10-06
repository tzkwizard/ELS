using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using MessageHandleApi.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using Microsoft.ServiceBus.Messaging;
using System.Diagnostics;
using FireSharp.Interfaces;
using FireSharp.Response;
using MessageHandleApi.Service;
using Microsoft.ServiceBus;
using Microsoft.WindowsAzure.Storage.File.Protocol;


namespace doucumentDB
{
    internal class Program
    {
        private static string EndpointUrl = "https://tstaz.documents.azure.com:443/";

        private static string AuthorizationKey =
            "6xPkxpC7FyiozobQOtQ8yFxbqd7uLOCz0pRo4i+GKxHdmISxDrMKZdaKQH0/0BJe/xC3UKdQM4C1x5d4Rxk3AQ==";

        private static string eventHubName = "eventhub1";

        private static string eventHubConnectionString =
            "Endpoint=sb://azrenweb-ns.servicebus.windows.net/;SharedAccessKeyName=get;SharedAccessKey=+mmaMKj+RjrCUMqC7bK1q4juLrxThN8FKnej026iEus=";

        private static void Main(string[] args)
        {
            try
            {
               //GetStartedDemo().Wait();
                Dichotomy.UpdateDcAll(EndpointUrl,AuthorizationKey).Wait();

                //Receive();               
                //Console.ReadLine();
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
                Console.ReadLine();
            }
        }

      

        private static void Receive()
        {
            string storageAccountName = "elsaotuo";
            string storageAccountKey =
                "AV49N0PZ1Qlz42b0w47EPoPbNLULgxYOWxsO4IvFmrAkZPzkdGCKKOJqyiHVGfAPex6HhkDSWpNQAIuPmBHBMA==";
            string storageConnectionString =
                string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}",
                    storageAccountName, storageAccountKey);

            string eventProcessorHostName = Guid.NewGuid().ToString();
            EventProcessorHost eventProcessorHost = new EventProcessorHost("1", eventHubName,
                EventHubConsumerGroup.DefaultGroupName, eventHubConnectionString, storageConnectionString);
            Console.WriteLine("Registering EventProcessor...");
            eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>(new EventProcessorOptions()
            {
                InitialOffsetProvider = (partitionId) => DateTime.UtcNow
            }).Wait();


            Console.WriteLine("Receiving. Press enter key to stop worker.");
            Console.ReadLine();
            eventProcessorHost.UnregisterEventProcessorAsync().Wait();
        }

        public static async Task<Database> GetDB(DocumentClient client)
        {
            Database database = client.CreateDatabaseQuery().Where(db => db.Id == "LMSRegistry")
                .AsEnumerable().FirstOrDefault()
                                ?? await client.CreateDatabaseAsync(
                                    new Database
                                    {
                                        Id = "LMSRegistry"
                                    });

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


        private static async Task GetStartedDemo()
        {
            // Create a new instance of the DocumentClient.
            DocumentClient client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            var database = await GetDB(client);
            var documentCollection = await GetDC(client, database);



            await DeleteAll(client, database);

            //await GetData(client, documentCollection);
            //ReadData(client, documentCollection);
            //await WriteData(client, documentCollection);
            Console.ReadLine();


            /*await client.DeleteDatabaseAsync(database.SelfLink);
             client.Dispose();*/
        }

        private static async Task DeleteAll(DocumentClient client, Database database)
        {
            IEnumerable<DocumentCollection> dz = client.CreateDocumentCollectionQuery(database.SelfLink)
                .AsEnumerable();

            foreach (var z in dz)
            {
                var families =
                    from f in client.CreateDocumentQuery(z.DocumentsLink)
                    select f;

                try
                {
                    foreach (var family in families)
                    {
                        var res = await client.DeleteDocumentAsync(family.SelfLink);
                        Console.WriteLine(family.SelfLink);
                    }
                }
                catch (Exception e)
                {
                    var zz = e.Message;
                    DeleteAll(client,database).Wait();
                }
            }
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
            /* var z=new Random();
            var topic=new Topic
            {
                Type = "Topic",
                Path = new PostPath
                {
                    District = "tst-azhang1",
                    School = "JP",
                    Classes = "Math",
                    Unit = "Calculus"
                },
                Info =new InfoPost
                {
                    teacher = "Newton",
                    start = "2015-4-1",
                    due = DateTime.Now.AddDays(z.Next(1,4)).ToString(CultureInfo.CurrentCulture),
                    description = "homework"
                }
            };
           var res= await client.CreateDocumentAsync(documentCollection.DocumentsLink, topic);*/


            var families =
                from f in client.CreateDocumentQuery<Topic>(documentCollection.DocumentsLink)
                where f.Type == "Topic"
                select f;
            foreach (var family in families)
            {
                var s = JsonConvert.SerializeObject(family);
                dynamic d = JsonConvert.DeserializeObject(s);
                var res = await client.DeleteDocumentAsync(family._self);
                Console.Write(d);
                //}
            }
        }


        private static async Task GetData(DocumentClient client, DocumentCollection documentCollection)
        {
            var t = (long) (DateTime.UtcNow.AddHours(-4).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;

            var families =
                from f in client.CreateDocumentQuery(documentCollection.DocumentsLink)
                select f;


            foreach (var family in families)
            {
                //if (Int64.Parse(family.Info.timestamp)>t)
                var res = await client.DeleteDocumentAsync(family.SelfLink);
                //Console.WriteLine(family._self);
                Console.WriteLine(family.SelfLink);
            }

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

            await client.CreateDocumentAsync(documentCollection.DocumentsLink, AndersenFamily);

            // Create the WakeField family document.
            Family WakefieldFamily = new Family
            {
                Id = "WakefieldFamily",
                Parents = new Parent[]
                {
                    new Parent {FamilyName = "Wakefield", FirstName = "Robin"},
                    new Parent {FamilyName = "Miller", FirstName = "Ben"}
                },
                Children = new Child[]
                {
                    new Child
                    {
                        FamilyName = "Merriam",
                        FirstName = "Jesse",
                        Gender = "female",
                        Grade = 8,
                        Pets = new Pet[]
                        {
                            new Pet {GivenName = "Goofy"},
                            new Pet {GivenName = "Shadow"}
                        }
                    },
                    new Child
                    {
                        FamilyName = "Miller",
                        FirstName = "Lisa",
                        Gender = "female",
                        Grade = 1
                    }
                },
                Address = new Address {State = "NY", County = "Manhattan", City = "NY"},
                IsRegistered = false
            };

            await client.CreateDocumentAsync(documentCollection.DocumentsLink, WakefieldFamily);
        }
    }
}