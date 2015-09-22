using System;
using System.IO;
using firebase1.DocumentDB;
using Microsoft.Azure.WebJobs;

namespace LMShandler
{
    public class Functions
    {
        private static IDocumentDB _IDocumentDb;

        static Functions()
        {
            _IDocumentDb = new firebase1.DocumentDB.DocumentDB();
        }

        [NoAutomaticTrigger]
        public static void ManualTrigger(TextWriter log)
        {
            log.WriteLine("Function is invoked at {0}", DateTime.Now);
            _IDocumentDb.UpdateDocument().Wait();
            log.WriteLine("Function is end at {0}", DateTime.Now);
        }
    }
}