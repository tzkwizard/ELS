using System;
using System.IO;
using LMS.service.Service;
using LMShandler.DocumentDB;
using Microsoft.Azure.WebJobs;

namespace LMShandler
{
    public class Functions
    {
        private static IDocumentDB _IDocumentDb;
        static Functions()
        {
            var iDbService=new DBService();
            _IDocumentDb = new DocumentDB.DocumentDB(iDbService);

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