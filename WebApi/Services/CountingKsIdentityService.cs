using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Services
{
    public class CountingKsIdentityService : ICountingKsIdentityService
    {
        public string CurrentUser
        {
            get { return "shawnwildermuth"; }
        }
    }
}