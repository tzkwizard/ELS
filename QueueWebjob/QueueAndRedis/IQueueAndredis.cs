using Microsoft.WindowsAzure.Storage.Queue;

namespace QueueWebjob.QueueAndRedis
{
    public interface IQueueAndredis
    {
        void InsertData(string message);
        void ProcessMessageAsync();
    }
}
