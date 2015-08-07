using Microsoft.WindowsAzure.Storage.Queue;

namespace WebJob.QueueAndRedis
{
    public interface IQueueAndredis
    {
        void InsertData(string message);
        void ProcessMessageAsync(CloudQueue queue);
        CloudQueue GetQueue();
    }
}
