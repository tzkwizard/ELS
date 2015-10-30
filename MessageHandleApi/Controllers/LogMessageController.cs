﻿using System;
using System.Web.Http;
using System.Web.Http.Cors;
using LMS.Common.Models.ELS;
using LMS.Common.Service;


namespace MessageHandleApi.Controllers
{
    [EnableCors("*", "*", "*")]
    public class LogMessageController : ApiController
    {
        private IELSService _iElsService;

        public LogMessageController(IELSService iElsService)
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
