using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage.Queue;

namespace MessageHandleApi.Service
{
    public interface IQueueService
    {
         Task SendToQueue(string m);
         CloudQueue GetQueue(string n);
    }
}