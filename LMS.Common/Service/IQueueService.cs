using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;

namespace LMS.Common.Service
{
    public interface IQueueService
    {
         Task SendToQueue(string m);
         CloudQueue GetQueue(string n);
    }
}