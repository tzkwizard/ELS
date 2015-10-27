using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;

namespace LMSqueue.ProcessMessage
{
    public interface IProcessMessage
    {
       Task SendMessageAsync();
       Task Send(string m);
    }
}
