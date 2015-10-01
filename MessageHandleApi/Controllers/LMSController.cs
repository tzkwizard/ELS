using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using MessageHandleApi.Models;
using MessageHandleApi.Service;
using Newtonsoft.Json;

namespace MessageHandleApi.Controllers
{
    [RoutePrefix("api/LMS")]
    public class LMSController : ApiController
    {
        private readonly IFirebaseClient _client;
        private readonly string _firebaseSecret;
        private IQueueService _iQueueService;
        private IDBService _iDbService;
        public LMSController(IQueueService iQueueService,IDBService iDbService)
        {
            _firebaseSecret = "F1EIaYtnYgfkVVI7sSBe3WDyUMlz4xV6jOrxIuxO";
            _iQueueService = iQueueService;
            _iDbService = iDbService;
            _client = _iDbService.GetFirebaseClient();
        }


        [HttpGet]
        public IHttpActionResult Get([FromUri] string url, int start)
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
                //FirebaseResponse response = await _client.PushAsync(message.url, message.body);
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
                //FirebaseResponse response = await _client.PushAsync(message.url, message.body);
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
                FirebaseResponse response = await _client.DeleteAsync(url);
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
                FirebaseResponse response = await _client.DeleteAsync(url);

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
                var tokenGenerator = new Firebase.TokenGenerator(_firebaseSecret);
                var authPayload = new Dictionary<string, object>()
                {
                        { "uid", u.uid },
                        { "user", u.name },
                        { "data", "here" }
                 };
                string token = tokenGenerator.CreateToken(authPayload);
                return Ok(token);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


    }
}
