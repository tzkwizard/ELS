using System;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Autofac;
using MessageHandleApi.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace doucumentDB
{
    internal class Program
    {
        private static string EndpointUrl = "https://tstaz.documents.azure.com:443/";

        private static string AuthorizationKey =
            "6xPkxpC7FyiozobQOtQ8yFxbqd7uLOCz0pRo4i+GKxHdmISxDrMKZdaKQH0/0BJe/xC3UKdQM4C1x5d4Rxk3AQ==";

        public static IPerson _p;

        private static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("haha");
                
                var builder = new ContainerBuilder();
                builder.RegisterType<Person>().As<IPerson>().InstancePerLifetimeScope();
                GetStartedDemo().Wait();
                //_p.Say("haha");
                //Console.ReadLine();
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
                Console.ReadLine();
            }
        }

        internal class tst
        {
            public tst(IPerson p)
            {
                _p = p;
            }
        }
        internal class Person:IPerson
        {
            public void Say(string m)
            {
                Console.WriteLine(m);
            }
        }

        internal interface IPerson
        {
            void Say(string m);
        }

        private static async Task<Database> GetDB(DocumentClient client)
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

        private static async Task<DocumentCollection> GetDC(DocumentClient client, Database database)
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

            await GetData(client, documentCollection);
            //ReadData(client, documentCollection);



            /*await client.DeleteDatabaseAsync(database.SelfLink);
             client.Dispose();*/
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
                dynamic document = new Document() { Id = "-JzLyG5lptD9dWAfMOWt" + i };
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


        private static async Task GetData(DocumentClient client, DocumentCollection documentCollection)
        {
            var t = (long)(DateTime.UtcNow.AddHours(-4).Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;

            var families =
                from f in client.CreateDocumentQuery(documentCollection.DocumentsLink)
                select f;



            foreach (var family in families)
            {
                //if (Int64.Parse(family.Info.timestamp)>t)
                var res=await client.DeleteDocumentAsync(family.SelfLink);
                //Console.WriteLine(family._self);

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