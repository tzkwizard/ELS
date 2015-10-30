using System;
using System.Threading.Tasks;
using System.Web.Http;
using Elasticsearch.Net.ConnectionPool;
using LMS.Common.Models.ELS;
using Nest;



namespace MessageHandleApi.Controllers
{
    [RoutePrefix("api/ElasticSearch")]
    public class ElasticSearchController : ApiController
    {
        private ElasticClient _client;
        private Uri _node;
        private ConnectionSettings _settings;
        private SniffingConnectionPool _connectionPool;


        public ElasticSearchController()
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

        [ESauth]
        [HttpPost]
        public async Task<IHttpActionResult> StringQuery([FromBody] QueryInfo info)
        {
           var es=new ESauth();
            try
            {
                var l = "";
                if (!string.IsNullOrEmpty(info.Lat) && !string.IsNullOrEmpty(info.Lon))
                {
                    l = info.Lat + "," + info.Lon;
                }
                var x = Filter<logs>.Range(t => t.OnField("@timestamp").Greater(info.Start).Lower(info.End));
                var y = Filter<logs>.GeoDistance("geoip.location", g => g.Distance(info.GeoDistance).Location(l));

                var resp = await _client.SearchAsync<logs>(s => s
                    //.Indices(new [] {"1","2"})
                    .Size(info.Size)
                    .Query(p => p.QueryString(q => q.Query(info.SearchText)))
                    //.Query(p => p.Term("logs.geoip.city_name.raw", "Beijing"))
                    //.Filter(f => f.Range(t => t.OnField("@timestamp").Greater(info.Start).Lower(info.End)))
                    .Filter(f => f.Bool(b => b
                        .Must(mf => mf.And(x && y))
                        )
                        )
                    );

                ELSresult result = new ELSresult
                {
                    Time = resp.ElapsedMilliseconds,
                    Total = (int)resp.HitsMetaData.Total,
                    MaxScore = resp.HitsMetaData.MaxScore,
                    Data = resp.Hits
                };


                //_logHandler.CallMessageApi(HttpContext.Current.Request, HttpContext.Current.Response);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest("stringquery" + e.Message);
            }

        }
        //[ESauth]
        [Route("SampleData")]
        [HttpPost]
        public async Task<IHttpActionResult> GetSampledata([FromBody] QueryInfo info)
        {
            try
            {
                /* var uri ="http://localhost:9200/";
                 var node = new Uri(uri);
                 var settings = new ConnectionSettings(node).SetBasicAuthentication("aotuo", "123456");;
                 var _client = new ElasticClient(settings);*/

                /*   Stream req = HttpContext.Current.Request.InputStream;
                   //req.Seek(0, System.IO.SeekOrigin.Begin);
                   string json = new StreamReader(req).ReadToEnd();
                   var input = JsonConvert.DeserializeObject<QueryInfo>(json);

                   var cx = HttpContext.Current.Request.Params.GetValues("Location");*/

                var l = "";
                if (!string.IsNullOrEmpty(info.Lat) && !string.IsNullOrEmpty(info.Lon))
                {
                    l = info.Lat + "," + info.Lon;
                }

                var x = Filter<logs>.Range(t => t.OnField("@timestamp").Greater(info.Start).Lower(info.End));
                var y = Filter<logs>.GeoDistance("geoip.location", g => g.Distance(info.GeoDistance).Location(l));

                var resp = await _client.SearchAsync<logs>(s => s
                    .Size(info.Size)
                    .Query(qq => qq.MatchAll())
                     .Filter(f => f.Bool(b => b
                        .Must(mf => mf.And(x && y))
                        )
                        )
                    );

                ELSresult result = new ELSresult
                {
                    Time = resp.ElapsedMilliseconds,
                    Total = (int)resp.HitsMetaData.Total,
                    MaxScore = resp.HitsMetaData.MaxScore,
                    Data = resp.Hits
                };
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest("SampleData " + e.Message);
            }
        }


        [ESauth]
        [Route("Term")]
        [HttpPost]
        public async Task<IHttpActionResult> TermQuery([FromBody] QueryInfo info)
        {
            try
            {
                var l = "";
                if (!string.IsNullOrEmpty(info.Lat) && !string.IsNullOrEmpty(info.Lon))
                {
                    l = info.Lat + "," + info.Lon;
                }

                var x = Filter<logs>.Range(t => t.OnField("@timestamp").Greater(info.Start).Lower(info.End));
                var y = Filter<logs>.GeoDistance("geoip.location", g => g.Distance(info.GeoDistance).Location(l));

                var resp = await _client.SearchAsync<logs>(s => s
                    .Size(info.Size)
                    .Query(qt => qt.Term(info.Field, info.SearchText))
                    .Filter(f => f.Bool(b => b
                        .Must(mf => mf.And(x && y))
                        )
                        )
                    );

                ELSresult result = new ELSresult
                {
                    Time = resp.ElapsedMilliseconds,
                    Total = (int)resp.HitsMetaData.Total,
                    MaxScore = resp.HitsMetaData.MaxScore,
                    Data = resp.Hits
                };
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest("term " + e.Message);
            }
        }


        [ESauth]
        [Route("TermBool")]
        [HttpPost]
        public async Task<IHttpActionResult> TermQueryWithBoolFilter([FromBody] QueryInfo info)
        {
            try
            {
                var l = "";
                if (!string.IsNullOrEmpty(info.Lat) && !string.IsNullOrEmpty(info.Lon))
                {
                    l = info.Lat + "," + info.Lon;
                }

                var x = Filter<logs>.Range(t => t.OnField("@timestamp").Greater(info.Start).Lower(info.End));
                var y = Filter<logs>.GeoDistance("geoip.location",
                    g => g.Distance(info.GeoDistance).Location(l));

                var fmust = x & y;
                var fnot = Filter<logs>.Not(fn => fn.Term("", ""));
                var fshould = Filter<logs>.MatchAll();
                if (info.Filterdata != null)
                {
                    foreach (var cc in info.Filterdata)
                    {

                        if (cc.Condition == "MUST")
                        {

                            var xx = Filter<logs>.Term(cc.Field, cc.Text);
                            fmust = fmust & xx;
                        }

                        if (cc.Condition == "MUST_NOT")
                        {

                            var yy = Filter<logs>.Term(cc.Field, cc.Text);
                            fnot = fnot & yy;
                        }
                        if (cc.Condition == "SHOULD")
                        {

                            var zz = Filter<logs>.Term(cc.Field, cc.Text);
                            fshould = fshould & zz;
                        }
                    }

                }

                var resp = await _client.SearchAsync<logs>(s => s
                    .Size(info.Size)
                    .Query(p => p.Term(info.Field, info.SearchText))
                    .Filter(f => f.Bool(b => b
                        .Must(mf => mf.And(fmust))
                        .MustNot(nf => nf.And(fnot))
                        .Should(sf => sf.And(fshould))
                        )
                    )
                    );

                ELSresult result = new ELSresult
                {
                    Time = resp.ElapsedMilliseconds,
                    Total = (int)resp.HitsMetaData.Total,
                    MaxScore = resp.HitsMetaData.MaxScore,
                    Data = resp.Hits
                };
                return Ok(result);

            }
            catch (Exception e)
            {
                return BadRequest("termbool " + e.Message);
            }
        }


        [ESauth]
        [Route("StringBool")]
        [HttpPost]
        public async Task<IHttpActionResult> StringQueryWithBoolFilter([FromBody] QueryInfo info)
        {
            try
            {
                var l = "";
                if (!string.IsNullOrEmpty(info.Lat) && !string.IsNullOrEmpty(info.Lon))
                {
                    l = info.Lat + "," + info.Lon;
                }

                var x = Filter<logs>.Range(t => t.OnField("@timestamp").Greater(info.Start).Lower(info.End));
                var y = Filter<logs>.GeoDistance("geoip.location", g => g.Distance(info.GeoDistance).Location(l));

                var fmust = x & y;
                var fnot = Filter<logs>.Not(fn => fn.Term("", ""));
                var fshould = Filter<logs>.MatchAll();
                if (info.Filterdata != null)
                {
                    foreach (var cc in info.Filterdata)
                    {

                        if (cc.Condition == "MUST")
                        {

                            var xx = Filter<logs>.Term(cc.Field, cc.Text);
                            fmust = fmust & xx;
                        }

                        if (cc.Condition == "MUST_NOT")
                        {

                            var yy = Filter<logs>.Term(cc.Field, cc.Text);
                            fnot = fnot & yy;
                        }
                        if (cc.Condition == "SHOULD")
                        {

                            var zz = Filter<logs>.Term(cc.Field, cc.Text);
                            fshould = fshould & zz;
                        }
                    }

                }

                var resp = await _client.SearchAsync<logs>(s => s
                    .Size(info.Size)
                    .Query(p => p.QueryString(q => q.Query(info.SearchText)))
                    .Filter(f => f.Bool(b => b
                        .Must(mf => mf.And(fmust))
                        .MustNot(nf => nf.And(fnot))
                        .Should(sf => sf.And(fshould))
                        )
                        )
                    );

                ELSresult result = new ELSresult
                {
                    Time = resp.ElapsedMilliseconds,
                    Total = (int)resp.HitsMetaData.Total,
                    MaxScore = resp.HitsMetaData.MaxScore,
                    Data = resp.Hits
                };
                return Ok(result);

            }
            catch (Exception e)
            {
                return BadRequest("stringbool " + e.Message);
            }
        }


    }
}
