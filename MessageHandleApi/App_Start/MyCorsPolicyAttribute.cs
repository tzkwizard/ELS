namespace MessageHandleApi.App_Start
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Cors;
    using System.Web.Http.Cors;

    /// <summary>
    /// This is used to setup the default cors policy for our site
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class MyCorsPolicyAttribute : Attribute, ICorsPolicyProvider
    {
        public Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains(CorsConstants.Origin))
            {
                return Task.FromResult(CreatePolicy(null));
            }

            var origin = request.Headers.GetValues(CorsConstants.Origin).FirstOrDefault() + "";

            return Task.FromResult(IsDomainValid(origin, "*.renweb.com") ? this.CreatePolicy(origin) : this.CreatePolicy(null));
        }

        private CorsPolicy CreatePolicy(string origin)
        {
            var policy = new CorsPolicy
            {
                AllowAnyMethod = true,
                AllowAnyHeader = true,
                SupportsCredentials = true
            };
            if (!String.IsNullOrWhiteSpace(origin))
            {
                policy.Origins.Add(origin);
            }
            return policy;
        }

        /// <summary>
        /// Checks if a wildcard string matches a domain
        /// </summary>
        public static bool IsDomainValid(string domain, string domainToCheck)
        {
            if (domainToCheck.Contains("*"))
            {
                string checkDomain = domainToCheck;
                if (checkDomain.StartsWith("*."))
                    checkDomain = "*" + checkDomain.Substring(2, checkDomain.Length - 2);
                return DoesWildcardMatch(domain, checkDomain);
            }
            else
            {
                return domainToCheck.Equals(domain, StringComparison.OrdinalIgnoreCase);
            }
        }
        /// <summary>
        /// Performs a wildcard (*) search on any string.
        /// </summary>
        public static bool DoesWildcardMatch(string originalString, string searchString)
        {
            originalString = originalString.ToLower();
            searchString = searchString.ToLower();
            if (!searchString.StartsWith("*"))
            {
                int stop = searchString.IndexOf('*');
                if (!originalString.StartsWith(searchString.Substring(0, stop)))
                    return false;
            }
            if (!searchString.EndsWith("*"))
            {
                int start = searchString.LastIndexOf('*') + 1;
                if (!originalString.EndsWith(searchString.Substring(start, searchString.Length - start)))
                    return false;
            }
            Regex regex = new Regex(searchString.Replace(@".", @"\.").Replace(@"*", @".*"));
            return regex.IsMatch(originalString);
        }
    }
}