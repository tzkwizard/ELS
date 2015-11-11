using System;
using System.Web.Http;
using System.Web.Http.Cors;
using LMS.Common.Models.ELS;
using LMS.Common.Service;
using LMS.Common.Service.Interface;


namespace MessageHandleApi.Controllers
{
    [EnableCors("*", "*", "*")]
    public class LogMessageController : ApiController
    {
        private IElsService _iElsService;

        public LogMessageController(IElsService iElsService)
        {
            _iElsService = iElsService;
        }
        public async void Post([FromBody]ELSLogs esLog)
        {
            var message = _iElsService.GetMessage(esLog);
            await _iElsService.SendQueueAsyncAll(message);
        }
        public string GetById(int id)
        {
            try
            {

                return "True";
            }
            catch (Exception)
            {

                return "false";
            }


        }

    }
}
