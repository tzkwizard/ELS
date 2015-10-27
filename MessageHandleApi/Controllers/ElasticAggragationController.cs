using System;
using System.Collections.Generic;
using System.Configuration;
using LMS.model.Models;
using Nest;
using System.Web.Http;
using Elasticsearch.Net.ConnectionPool;
using System.Threading.Tasks;
using LMS.model.Models.Api;
using LMS.model.Models.ELS;


namespace MessageHandleApi.Controllers
{
    [RoutePrefix("api/ElasticAggragation")]
    public class ElasticAggragationController : ApiController
    {
        private ElasticClient _client;
        private Uri _node;
        private ConnectionSettings _settings;
        private SniffingConnectionPool _connectionPool;
        public ElasticAggragationController()
        {
            _client = GetESclient();
        }



        private ElasticClient GetESclient()
        {

            const string elUri = "http://localhost:9200/";
            //const string elUri = "http://191.238.179.97:9200/";
            //var uri = ConfigurationManager.AppSettings["Local"];
            _node = new Uri(elUri);
            _connectionPool = new SniffingConnectionPool(new[] { _node });
            _settings = new ConnectionSettings(_connectionPool).SetBasicAuthentication("aotuo", "123456");
            _client = new ElasticClient(_settings);
            return _client;
        }


       
        [Route("Term")]
        [HttpPost]
        public async Task<IHttpActionResult> TermAggragation([FromBody] QueryInfo info)
        {
            try
            {
                var resp = await _client.SearchAsync<logs>(s => s
                        .Size(info.Size)
                        .Aggregations(fa => fa
                            .Filter("ff", f => f
                                .Filter(fd => fd
                                    .Range(t => t
                                        .OnField("@timestamp").Greater(info.Start).Lower(info.End)))
                                        .Aggregations(a => a.Terms("agg", ag => ag.Field(info.AggField).Size(info.SubSize)))

                                         )
                                        )

                        );
                var ff = resp.Aggs.Filter("ff");
                var fagg = ff.Terms("agg");

                ELSresult result = new ELSresult
                {
                    Time = resp.ElapsedMilliseconds,
                    Document = (int)resp.HitsMetaData.Total,
                    Total = (int)ff.DocCount,
                    AggData = fagg.Items,
                    AggTotal = fagg.Items.Count
                };
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [Route("TermQuery")]
        [HttpPost]
        public async Task<IHttpActionResult> TermQueryAggragation([FromBody] QueryInfo info)
        {
            try
            {
                var resp = await _client.SearchAsync<logs>(s => s
                    //.Query(p => p.Term("geoip.country_code3.raw", "USA"))
                        .Query(p => p.Term(info.Field, info.SearchText))
                        .Aggregations(fa => fa
                            .Filter("ff", f => f
                                .Filter(fd => fd
                                    .Range(t => t
                                        .OnField("@timestamp").Greater(info.Start).Lower(info.End)))
                                        .Aggregations(a => a.Terms("agg", ag => ag.Field(info.AggField).Size(info.SubSize)))

                                         )
                                        )

                        );
                var ff = resp.Aggs.Filter("ff");
                var fagg = ff.Terms("agg");

                ELSresult result = new ELSresult
                {
                    Time = resp.ElapsedMilliseconds,
                    Document = (int)resp.HitsMetaData.Total,
                    Total = (int)ff.DocCount,
                    AggData = fagg.Items,
                    AggTotal = fagg.Items.Count
                };
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [Route("StringQuery")]
        [HttpPost]
        public async Task<IHttpActionResult> TermAggragationwithQuery([FromBody] QueryInfo info)
        {
            try
            {
                var resp = await _client.SearchAsync<logs>(s => s
                         .Query(p => p.QueryString(qs => qs.Query(info.SearchText)))
                        .Aggregations(fa => fa
                            .Filter("ff", f => f
                                .Filter(fd => fd
                                    .Range(t => t
                                        .OnField("@timestamp").Greater(info.Start).Lower(info.End)))
                                        .Aggregations(a => a.Terms("agg", ag => ag.Field(info.AggField).Size(info.SubSize)))

                                         )
                                        )

                        );
                var ff = resp.Aggs.Filter("ff");
                var fagg = ff.Terms("agg");

                ELSresult result = new ELSresult
                {
                    Time = resp.ElapsedMilliseconds,
                    Document = (int)resp.HitsMetaData.Total,
                    Total = (int)ff.DocCount,
                    AggData = fagg.Items,
                    AggTotal = fagg.Items.Count
                };
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [Route("DateHistogram")]
        [HttpPost]
        public async Task<IHttpActionResult> DateHistogramAggregation([FromBody] QueryInfo info)
        {
            try
            {
                var resp = await _client.SearchAsync<logs>(s => s
                    .Aggregations(fa => fa
                        .Filter("ff", f => f
                            .Filter(fd => fd.Range(t => t.OnField("@timestamp").Greater(info.Start).Lower(info.End)))
                            .Aggregations(a => a.DateHistogram("agg", ag => ag.Field("@timestamp").Interval(info.Span)))
                            )));
                var ff = resp.Aggs.Filter("ff");
                var fagg = ff.DateHistogram("agg");

                ELSresult result = new ELSresult
                {
                    Time = resp.ElapsedMilliseconds,
                    Document = (int)resp.HitsMetaData.Total,
                    Total = (int)ff.DocCount,
                    AggTotal = fagg.Items.Count,
                    DateHistData = fagg.Items
                };
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [Route("DashBoardPie")]
        [HttpPost]
        public async Task<IHttpActionResult> DashboardPieAggregation([FromBody] QueryInfo info)
        {
            try
            {
                var n = 1;
                var result = new Dictionary<int, IList<KeyItem>>();

                foreach (var agg in info.MultiField)
                {
                    var resp = await _client.SearchAsync<logs>(s => s
                        .Aggregations(fa => fa
                            .Filter("ff", f => f
                                .Filter(fd => fd
                                    .Range(t => t
                                        .OnField("@timestamp").Greater(info.Start).Lower(info.End)))
                                .Aggregations(a => a.Terms("agg", ag => ag.Field(agg).Size(info.SubSize)))

                            )
                        ));
                    var ff = resp.Aggs.Filter("ff");
                    var fagg = ff.Terms("agg");


                    result.Add(n, fagg.Items);
                    n++;
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [Route("GeoDistance")]
        [HttpPost]
        public async Task<IHttpActionResult> GeoDistanceAggragation([FromBody] QueryInfo info)
        {
            try
            {
                var l = "";
                if (info.Location.Lat != "" && info.Location.Lon != "")
                {
                    l = info.Location.Lat + "," + info.Location.Lon;
                }
                var resp = await _client.SearchAsync<logs>(s => s
                     .Aggregations(fa => fa
                         .Filter("ff", f => f
                             .Filter(fd => fd
                                 .Range(t => t
                                     .OnField("@timestamp").Greater(info.Start).Lower(info.End)))
                                     .Aggregations(a => a
                                            .GeoDistance("geo", g => g
                                            .Field("geoip.location")
                                               .Origin(l).Unit(GeoUnit.Miles)
                                               .Ranges(
                                                    r => r.To(1000),
                                                     r => r.From(1000).To(3000),
                                                            r => r.From(3000)
                                                            ))

                                      )
                                     ))

                     );
                var ff = resp.Aggs.Filter("ff");
                var fagg = ff.GeoDistance("geo");

                ELSresult result = new ELSresult
                {
                    Time = resp.ElapsedMilliseconds,
                    Document = (int)resp.HitsMetaData.Total,
                    Total = (int)ff.DocCount,
                    GeoAggdata = fagg.Items,
                    AggTotal = fagg.Items.Count
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