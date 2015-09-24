using System.Threading.Tasks;

namespace LMShandler.DocumentDB
{
    public interface IDocumentDB
    {
        Task UpdateDocument();
    }
}
