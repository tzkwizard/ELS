using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MessageHandleApi.Service
{
    public interface IQueueService
    {
         Task SendToQueue(string m);
    }
}