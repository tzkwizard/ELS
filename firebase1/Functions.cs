using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.WindowsAzure.Storage.Queue.Protocol;
using Newtonsoft.Json;
using firebase1.DocumentDB;


namespace firebase1
{
    public class Functions
    {
        private static IDocumentDB _IDocumentDb;

        static Functions()
        {
            _IDocumentDb = new DocumentDB.DocumentDB();
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