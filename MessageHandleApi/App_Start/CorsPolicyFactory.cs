namespace MessageHandleApi.App_Start
{
    using System.Net.Http;
    using System.Web.Http.Cors;

    public class CorsPolicyFactory : ICorsPolicyProviderFactory
    {
        private ICorsPolicyProvider _provider = new MyCorsPolicyAttribute();

        public ICorsPolicyProvider GetCorsPolicyProvider(HttpRequestMessage request)
        {
            return _provider;
        }
    }
}