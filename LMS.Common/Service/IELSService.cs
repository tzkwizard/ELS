using System;
using System.Threading.Tasks;
using LMS.Common.Models.ELS;

namespace LMS.Common.Service
{
    public interface IELSService
    {
        Task SendQueueAsyncAll(string m);
        String GetMessage(ELSLogs esLog);
    }
}