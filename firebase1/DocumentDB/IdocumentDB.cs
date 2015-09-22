using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;

namespace firebase1.DocumentDB
{
    public interface IDocumentDB
    {
        Task UpdateDocument();
    }
}
