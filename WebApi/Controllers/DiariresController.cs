using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.App_Data;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    public class DiariresController : BaseApiController
    {
        private ICountingKsIdentityService _identityService;

        public DiariresController(ICountingKsRepository repo,ICountingKsIdentityService identityService):base(repo)
        {
         _identityService  = identityService;
        }


        public IEnumerable<DiaryModel> Get()
        {
            var username = _identityService.CurrentUser;
            var res = TheRepository.GetDiaries(username)
                .OrderByDescending(d => d.CurrentDate)
                .Take(10)
                .ToList()
                .Select(d => TheModelFactory.Create(d));
            return res;
        }

        public HttpResponseMessage Get(DateTime diaryid)
        {
             var username = _identityService.CurrentUser;
            var res = TheRepository.GetDiary(username,diaryid);
            if (res == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }


            return Request.CreateResponse(HttpStatusCode.OK, TheModelFactory.Create(res));
        }
    }
}
