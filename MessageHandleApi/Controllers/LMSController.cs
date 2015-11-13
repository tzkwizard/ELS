using System;
using System.Threading.Tasks;
using System.Web.Http;
using FireSharp.Response;
using LMS.Common.Models.Api;
using LMS.Common.Service;
using LMS.Common.Service.Interface;

namespace MessageHandleApi.Controllers
{
    [RoutePrefix("api/LMS")]
    public class LmsController : ApiController
    {
        private readonly IAzureStorageService _iAzureStorageService;
        private readonly ILmsDashboardService _iLmsDashboardService;
        private readonly IDbService _iDbService;

        public LmsController(IDbService iDbService,
            IAzureStorageService iAzureStorageService)
        {
            _iDbService = iDbService;
            _iAzureStorageService = iAzureStorageService;
            _iLmsDashboardService = new LmsDashboardService(iDbService);
        }


        [HttpGet]
        public IHttpActionResult Get([FromUri] string url, long start)
        {
            try
            {
                var res = (start == 0)
                    ? _iLmsDashboardService.GetList(url)
                    : _iLmsDashboardService.GetMoreList(url, start);
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
                var res = _iLmsDashboardService.GetCalendar();
                return Ok(res);
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
                string token = _iDbService.FBoperation().GetFirebaseToken(u.uid, u.name, "something");
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
                var res = _iAzureStorageService.SearchChat(roomId, start);
                return Ok(res);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}