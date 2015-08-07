using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace MessageHandleApi
{
    public class ESauth : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {            
            try
            {
           
                var token = HttpContext.Current.Request.Params.GetValues("token");
                if (token != null)
                {
                    token[0] = EsCipher.Decrypt(token[0], "Elastic");
                    if (token[0] != "binggo")
                    {
                        actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Not Access to Elasticsearch");
                    }
                }
                else
                {
                    actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Not Access to Elasticsearch");
                }
            }
            catch (Exception e)
            {
                actionContext.Response = actionContext.ControllerContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, e.Message);
            }

        }

        /*  public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            // 1. Look for credentials in the request.
            HttpRequestMessage request = context.Request;
            AuthenticationHeaderValue authorization = request.Headers.Authorization;
            var token = HttpContext.Current.Request.Params.GetValues("token");

            if (token == null || token[0] != "binggo")
            {
                return request.CreateErrorResponse(HttpStatusCode.Unauthorized, "error token");
            }

            return request.CreateResponse(HttpStatusCode.OK);
        }*/

    }
}