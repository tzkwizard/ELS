using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.App_Data;

namespace WebApi.Controllers
{

    [RoutePrefix("api/stats")]
    public class StatsController : BaseApiController
    {
        public StatsController(ICountingKsRepository repo) : base(repo)
        {
            
        }

        [Route("")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            var result = new
            {
                NumFoods=TheRepository.GetAllFoods().Count(),
                NumUsers=TheRepository.GetApiUsers().Count()
            };
            return Ok(result);
        }

        [Route("~/api/Stat/{id:int}")]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            if (id%2 == 0)
            {
                return Ok(new {NumFoods = TheRepository.GetAllFoods().Count()});
            }
            else
            {
               return NotFound();
            }
        }



    }
}
