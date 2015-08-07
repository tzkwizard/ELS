using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.App_Data;
using WebApi.App_Data.Entities;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class MeasuresController : BaseApiController
    {
         public MeasuresController(ICountingKsRepository repo):base(repo)
        {
            
        }

        public IEnumerable<MeasureModel> Get(int foodid)
        {
            var res = TheRepository.GetMeasuresForFood(foodid)
                .ToList()
                .Select(m => TheModelFactory.Create(m));

            return res;
        }

        public MeasureModel Get(int foodid, int id)
        {
            var res = TheRepository.GetMeasure(id);
            if (res.Food.Id==foodid)
            {
                return TheModelFactory.Create(res);
            }
            return null;

        }
       
    }
}
