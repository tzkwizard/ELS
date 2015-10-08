using System;

namespace doucumentDB
{
    public class Program
    {
        private static void Main(string[] args)
        {
            try
            {

                BackupPost.BackupPostAll().Wait();
                //Batch.RunBatch(args);
                //DocumentDB.GetStartedDemo().Wait();
                //Dichotomy.UpdateDcAll(EndpointUrl,AuthorizationKey).Wait();
                //ChatBackup.TableDemo();
                //EventHub.Receive();               
                //Console.ReadLine();
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
                Console.ReadLine();
            }
        }


    }
}