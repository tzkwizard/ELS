using LMS.Common.Models.Api;

namespace LMS.Common.Service
{
    public interface IAzureStorageService
    {
        LMSChatresult SearchChat(string roomId, long start);
    }
}