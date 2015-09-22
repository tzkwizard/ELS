using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MessageHandleApi.Models;

namespace MessageHandleApi.Service
{
    public interface IELSService
    {
        Task SendQueueAsyncAll(string m);
        String GetMessage(ELSLogs esLog);
    }
}