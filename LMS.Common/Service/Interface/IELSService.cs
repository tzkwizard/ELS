using System;
using System.Threading.Tasks;
using LMS.Common.Models.ELS;

namespace LMS.Common.Service.Interface
{
    public interface IElsService
    {
        Task SendQueueAsyncAll(string m);
        String GetMessage(ELSLogs esLog);
    }
}