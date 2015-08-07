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
    public class MeasuresV2Controller : BaseApiController
    {
         public MeasuresV2Controller(ICountingKsRepository repo):base(repo)
        {
            
        }

        public IEnumerable<MeasureV2Model> Get(int foodid)
        {
            var res = TheRepository.GetMeasuresForFood(foodid)
                .ToList()
                .Select(m => TheModelFactory.Create2(m));

            return res;
        }



        public MeasureV2Model Get(int foodid, int id)
        {
            var res = TheRepository.GetMeasure(id);
            if (res.Food.Id==foodid)
            {
                return TheModelFactory.Create2(res);
            }
            return null;

        }
       
    }
}
