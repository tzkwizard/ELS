using System;
using System.Collections;
using System.Threading.Tasks;
using System.Web.Http;
using Elasticsearch.Net.ConnectionPool;
using LMS.Common.Models.ELS;
using Nest;

namespace MessageHandleApi.Controllers
{
    [RoutePrefix("api/ElasticMapping")]
    public class ElasticMappingController : ApiController
    {
        private ElasticClient _client;
        private Uri _node;
        private ConnectionSettings _settings;
        private SniffingConnectionPool _connectionPool;

        public ElasticMappingController()
        {
            GetESclient();
        }


        private void GetESclient()
        {
            const string elUri = "http://localhost:9200/";
            //const string elUri = "http://191.238.179.97:9200/";
            //var uri = ConfigurationManager.AppSettings["Local"];
            _node = new Uri(elUri);
            _connectionPool = new SniffingConnectionPool(new[] { _node });
            _settings = new ConnectionSettings(_connectionPool).SetBasicAuthentication("aotuo", "123456");
            _client = new ElasticClient(_settings);
        }


        [Route("Login/{id:length(1,20)}/{pass:length(1,20)}")]
        [HttpGet]
        public IHttpActionResult CheckId(string id, string pass)
        {
            try
            {
                const string elUri = "http://localhost:9200/";
                //var uri = ConfigurationManager.AppSettings["Local"];
                _node = new Uri(elUri);
                var connectionPool = new SniffingConnectionPool(new[] { _node });
                _settings = new ConnectionSettings(_node).SetBasicAuthentication(id, pass);
                _client = new ElasticClient(_settings);
                //if (_client.RootNodeInfo().Status != 200) return BadRequest("error identity");
                if (id != "aotuo" || pass != "123456") return BadRequest("error identity"); //temporary check
                var res = EsCipher.Encrypt("binggo", "Elastic");
                return Ok(res);
            }
            catch (Exception e)
            {
                return BadRequest("Error " + e.Message);
            }
            // System.Web.Http.Results
        }

        [ESauth]
        [Route("LogstashMap")]
        [HttpGet]
        public async Task<IHttpActionResult> Map()
        {
            try
            {
                var result = await _client.GetFieldMappingAsync<logs>(m => m.Fields("*"));
                var index = new ArrayList();
                var field = new ArrayList();
                var flag = true;

                foreach (var i in result.Indices)
                {
                    if (flag)
                    {
                        var ti = i.Value.Mappings["logs"].Values;
                        foreach (var fi in ti)
                        {
                            field.Add(fi.FullName);
                        }
                        flag = false;
                    }
                    index.Add(i.Key);
                }
                ELSmap map = new ELSmap { Index = index, Field = field };

                return Ok(map);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [ESauth]
        [Route("AutoFill")]
        [HttpPost]
        public async Task<IHttpActionResult> AutoFill([FromBody] QueryInfo info)
        {
            //var autoField = new ArrayList() { "ident.raw", "auth.raw", "geoip.city_name.raw","geoip.real_region_name.raw", "response","clientip.raw","geoip.country_code3.raw",
            // "geoip.country_name.raw", "geoip.region_name.raw", "geoip.postal_code.raw"};
            var autoField = new ArrayList() { 
    "geoip.postal_code.raw",
    "geoip.country_name.raw",
    "ident.raw",
    "clientip.raw",
    "geoip.region_name.raw",
    "auth.raw",
    "request.raw",
    "agent.raw",
    "geoip.continent_code.raw",
    "geoip.city_name.raw",
    "geoip.continent_code.raw",
    "geoip.country_code2.raw",
    "geoip.country_code3.raw",
    "response.raw",
    "geoip.region_name.raw",
    "geoip.country_name.raw",
    "APIresponse.raw",
    "geoip.real_region_name.raw",
    "verb.raw",
    "action.raw"
            };
            try
            {
                var res = new ArrayList();

                foreach (var af in autoField)
                {  
                    var x = Query<logs>.Prefix(af.ToString().Substring(0, af.ToString().Length - 4), info.SearchText);
                    var y = Query<logs>.Prefix(af.ToString(), info.SearchText);                   
                    var resp = await _client.SearchAsync<logs>(s => s
                        //.Query(p => p.Prefix(af.ToString(), info.SearchText)).Analyzer("simple")
                       .Query(p => p.Bool(b => b.Should(x||y)))
                       .Aggregations(fa => fa
                          .Filter("ff", f => f
                              .Filter(fd => fd
                                  .Range(t => t
                                      .OnField("@timestamp").Greater(info.Start).Lower(info.End)))
                                      .Aggregations(a => a.Terms("agg", ag => ag.Field(af.ToString()).Size(info.SubSize)))
                                       )
                                      )
                      );
                    //time += resp.ElapsedMilliseconds;
                    var ff = resp.Aggs.Filter("ff");


                    var fagg = ff.Terms("agg");
                    foreach (var item in fagg.Items)
                    {
                        res.Add(item.Key);
                    }
                }

                ELSresult result = new ELSresult
                {

                    Total = res.Count,
                    AutoData = res
                };
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}