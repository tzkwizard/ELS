using System;
using System.Threading.Tasks;
using LMS.model.Models;

namespace LMS.service.Service
{
    public interface IELSService
    {
        Task SendQueueAsyncAll(string m);
        String GetMessage(ELSLogs esLog);
    }
}