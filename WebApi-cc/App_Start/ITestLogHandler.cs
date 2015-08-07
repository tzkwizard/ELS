using System.Web;
namespace WebApi
{
    public interface ITestLogHandler
    {
      
        void Save(string ipAddress, HttpContext context, HttpRequest request, HttpResponse response);
        void Update(int responselog);
        void SendQueueAsyncAll(int num);
        void Listqueue();
    }
}
