using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using FireSharp.Interfaces;
using FireSharp.Response;
using LMS.model.Models;
using LMS.model.Models.Api;
using LMS.service.Service;
using Newtonsoft.Json;

namespace MessageHandleApi.Controllers
{
    [RoutePrefix("api/LMS")]
    public class LMSController : ApiController
    {
        private IQueueService _iQueueService;
        private IDbService _iDbService;
        private IAzureStorageService _iAzureStorageService;
        public LMSController(IQueueService iQueueService, IDbService iDbService, IAzureStorageService iAzureStorageService)
        {
            _iQueueService = iQueueService;
            _iDbService = iDbService;
            _iAzureStorageService = iAzureStorageService;
        }


        [HttpGet]
        public IHttpActionResult Get([FromUri] string url, long start)
        {
            try
            {
                var res = (start == 0) ? _iDbService.GetList(url) : _iDbService.GetMoreList(url,start);
                //FirebaseResponse response = await _client.GetAsync(url);
                //dynamic ds = JsonConvert.DeserializeObject(response.Body);
                //return Ok(ds);
                return Ok(res);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("calendar")]
        public IHttpActionResult GetCalendar([FromUri] string url)
        {
            try
            {
                var res = _iDbService.GetCalendar();
                return Ok(res);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }



        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] LMSMessage message)
        {
            try
            {
                message.body.timestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
                string m = JsonConvert.SerializeObject(message);
                await _iQueueService.SendToQueue(m);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPost]
        [Route("comment")]
        public async Task<IHttpActionResult> PostC([FromBody] LMSMessage message)
        {
            try
            {
                message.body.timestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
                string m = JsonConvert.SerializeObject(message);
                await _iQueueService.SendToQueue(m);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpDelete]
        public async Task<IHttpActionResult> Delete([FromUri] string url)
        {
            try
            {
                var client = _iDbService.GetFirebaseClient();
                FirebaseResponse response = await client.DeleteAsync(url);
                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }


        [HttpDelete]
        [Route("comment")]
        public async Task<IHttpActionResult> DeleteC([FromUri] string url)
        {
            try
            {
                var client = _iDbService.GetFirebaseClient();
                FirebaseResponse response = await client.DeleteAsync(url);

                return Ok(response);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpPost]
        [Route("token")]
        public IHttpActionResult PostT([FromBody] User u)
        {
            try
            {
                string token = _iDbService.GetFirebaseToken(u.uid, u.name, "something");
                return Ok(token);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("chat")]
        public IHttpActionResult GetChat([FromUri] string roomId, long start)
        {
            try
            {
                var res=_iAzureStorageService.SearchChat(roomId,start);
                return Ok(res);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
