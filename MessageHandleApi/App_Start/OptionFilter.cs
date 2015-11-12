using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MessageHandleApi
{
    public class OptionFilter : DelegatingHandler
    {
        //private LogHandler _logHandler;

        public OptionFilter()
        {
            //_logHandler = new LogHandler();
        }
        protected async override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method.Method == "OPTIONS")
            {
                return request.CreateResponse(HttpStatusCode.OK);
            }

            //_logHandler.CallMessageApi(HttpContext.Current.Request, HttpContext.Current.Response); 
             Debug.WriteLine("Process request");
             // Call the inner handler.
             var response = await base.SendAsync(request, cancellationToken);
             Debug.WriteLine("Process response");
             return response;
        }

    }
}