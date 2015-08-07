using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.App_Data;
using WebApi.Models;
using WebApi.Services;
using System.Web.Mvc;

namespace WebApi.Controllers
{ 
    public class DiariresEntriesController : BaseApiController
    {
         private ICountingKsIdentityService _identityService;

        public DiariresEntriesController(ICountingKsRepository repo,ICountingKsIdentityService identityService):base(repo)
        {
         _identityService  = identityService;
        }

        
       
        public IEnumerable<DiaryEntryModel> Get(DateTime diaryid)
        {
            var username = _identityService.CurrentUser;
            var res = TheRepository.GetDiaryEntries(_identityService.CurrentUser,diaryid.Date)
                .ToList()
                .Select(d => TheModelFactory.Create(d));
            return res;
        }

        public HttpResponseMessage Get(DateTime diaryid, int id)
        {
             
            var res = TheRepository.GetDiaryEntry(_identityService.CurrentUser, diaryid.Date, id);

            if (res == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }


            return Request.CreateResponse(HttpStatusCode.OK, TheModelFactory.Create(res));
        }


        public HttpResponseMessage Post(DateTime diaryid, [FromBody]DiaryEntryModel model)
        {
            try
            {
                var entity = TheModelFactory.Parse(model);
                if (entity == null)
                    Request.CreateErrorResponse(HttpStatusCode.BadRequest, "cannot found entry");
                var diary = TheRepository.GetDiary(_identityService.CurrentUser,diaryid);
                if (diary == null)
                    Request.CreateResponse(HttpStatusCode.NotFound);
                //check duplicate
                if (diary.Entries.Any(e => e.Measure.Id == entity.Measure.Id))
                {  return Request.CreateResponse(HttpStatusCode.BadRequest, "duplicate");}

                //save new entry
                diary.Entries.Add(entity);
                if (TheRepository.SaveAll())
                {
             //  LogActivity.LogSend("post",Request);
                    return Request.CreateResponse(HttpStatusCode.OK, TheModelFactory.Create(entity));
                    
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "not added");
                }
            }
            catch(Exception exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, exception);
            }
        }


        public HttpResponseMessage Delete(DateTime diaryid,int id)
        {
            try
            {
                if (TheRepository.GetDiaryEntries(_identityService.CurrentUser, diaryid).Any(e => e.Id == id)==false)
                {
                   return Request.CreateResponse(HttpStatusCode.NotFound,"not found");
                }
                if (TheRepository.DeleteDiaryEntry(id) && TheRepository.SaveAll())
                {
                   // LogActivity.LogSend("delete", Request);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                  return  Request.CreateResponse(HttpStatusCode.BadRequest,"not delete");
                }

            }
            catch (Exception exception)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, exception);
            }
        }


        public HttpResponseMessage Patch(DateTime diaryid, int id, [FromBody] DiaryEntryModel model)
        {
            try
            {
                var entity = TheRepository.GetDiaryEntry(_identityService.CurrentUser, diaryid,id);
                if (entity == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                var parsedValue = TheModelFactory.Parse(model);
                if (parsedValue == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "not valid value");
                }
                if (entity.Quantity != parsedValue.Quantity)
                {
                    entity.Quantity = parsedValue.Quantity;
                    if (TheRepository.SaveAll())
                    {
                        //LogActivity.LogSend("patch", Request);
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                }
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest,"fail");

            }
            catch (Exception e)
            {

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }
    }
}
