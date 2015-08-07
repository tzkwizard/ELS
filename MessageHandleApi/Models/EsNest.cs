using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nest;

namespace MessageHandleApi.Models
{
    public class EsNest
    {
        public ElasticClient Client { set; get; }
    }
}