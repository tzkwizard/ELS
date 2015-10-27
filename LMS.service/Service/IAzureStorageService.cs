using System.Collections.Generic;
using System.Threading.Tasks;
using LMS.model.Models.Api;
using Microsoft.WindowsAzure.Storage.Queue;

namespace LMS.service.Service
{
    public interface IAzureStorageService
    {
        LMSChatresult SearchChat(string roomId, long start);
    }
}