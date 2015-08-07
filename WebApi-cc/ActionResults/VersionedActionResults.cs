using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace WebApi.ActionResults
{
    public class VersionedActionResults<T> : IHttpActionResult
    {
        private HttpRequestMessage _request;
        private string _version;
        private T _body;

        public VersionedActionResults(HttpRequestMessage request, string version, T body)
        {
            _request = request;
            _version = version;
            _body = body;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var msg = _request.CreateResponse(_body);
            msg.Headers.Add("XXX-Ourversion",_version);
            return Task.FromResult(msg);
        }
    }
}