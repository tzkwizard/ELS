using System;
using System.Threading.Tasks;
using LMS.model.Models;
using LMS.model.Models.ELS;

namespace LMS.service.Service
{
    public interface IELSService
    {
        Task SendQueueAsyncAll(string m);
        String GetMessage(ELSLogs esLog);
    }
}