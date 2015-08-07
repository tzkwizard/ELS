using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net.ConnectionPool;
using MessageHandleApi.Models;
using Nest;

namespace MessageHandleApi.Test.Controllers
{
    class TestData
    {
        private static ElasticClient _client;
        public static void GetClient()
        {
            const string elUri = "http://localhost:9200/";
            //const string elUri = "http://191.238.179.97:9200/";

            var _node = new Uri(elUri);
            var connectionPool = new SniffingConnectionPool(new[] { _node });
            var _settings = new ConnectionSettings(_node).SetBasicAuthentication("aotuo", "123456");
            _client = new ElasticClient(_settings);
        }

        public static List<QueryInfo> GetTestProducts()
        {
            var testProducts = new List<QueryInfo>();

            FilterInfo[] fd = new FilterInfo[]
            {
              new FilterInfo { Text = "Tzkwizard", Field = "ident.raw", Condition = "MUST" },
              new FilterInfo { Text = "POST", Field = "verb.raw", Condition = "MUST_NOT" },
              new FilterInfo { Text = "GET", Field = "verb.raw", Condition = "SHOULD" }
            };


            testProducts.Add(new QueryInfo
            {
                Size = 1,
                Field = "geoip.city_name",
                AggField = "geoip.real_region_name.raw",
                SubSize = 20,
                Start = DateTime.Now.AddMonths(-5),
                End = DateTime.Now,
                SearchText = "BingLi",
                Span = "day",
                Location = new Coordinates()
                {
                    Lat = "44",
                    Lon = "44"
                },
                Lon = "44",
                Lat = "44",
                GeoDistance = "500mi",
                MultiField = new string[] { "ident.raw", "verb.raw" },
                Filterdata = fd

            });
            testProducts.Add(new QueryInfo
            {
                Size = 2,
                Field = "geoip.city_name",
                AggField = "geoip.real_region_name.raw",
                SubSize = 20,
                Start = DateTime.Now.AddMonths(-5),
                End = DateTime.Now,
                SearchText = "BingLi",
                Span = "day",
                Location = new Coordinates()
                {
                    Lat = "44",
                    Lon = "44"
                },
                Lon = "44",
                Lat = "44",
                GeoDistance = "500mi",
                MultiField = new string[] { "ident.raw", "verb.raw" },
                Filterdata = fd
            });
            testProducts.Add(new QueryInfo
            {
                Size = 3,
                Field = "geoip.city_name",
                AggField = "geoip.real_region_name.raw",
                SubSize = 20,
                Start = DateTime.Now.AddMonths(-5),
                End = DateTime.Now,
                SearchText = "BingLi",
                Span = "day",
                Location = new Coordinates()
                {
                    Lat = "44",
                    Lon = "44"
                },
                Lon = "44",
                Lat = "44",
                GeoDistance = "500mi",
                MultiField = new string[] { "ident.raw", "verb.raw" },
                Filterdata = fd
            });

            return testProducts;
        }
    }
}
