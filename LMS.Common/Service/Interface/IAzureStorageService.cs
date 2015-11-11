using LMS.Common.Models.Api;

namespace LMS.Common.Service.Interface
{
    public interface IAzureStorageService
    {
        LMSChatresult SearchChat(string roomId, long start);
    }
}