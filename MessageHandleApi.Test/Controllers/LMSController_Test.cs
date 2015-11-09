using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using LMS.Common.Models.Api;
using LMS.Common.Models.ELS;
using LMS.Common.Service;
using MessageHandleApi.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;

namespace MessageHandleApi.Test.Controllers
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class LMSControllerTest
    {
        private static IQueueService _iQueueService;
        private static IDbService _iDbService;
        private static IAzureStorageService _iAzureStorageService;
        private static LMSController _controller;

        public LMSControllerTest()
        {
            _iQueueService = new QueueService();
            _iDbService = new DbService();
            _iAzureStorageService = new AzureStorageService();
            _controller = new LMSController(_iQueueService, _iDbService, _iAzureStorageService);
        }

        [TestMethod]
        public void Get_Test()
        {
            var time = (long) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;

            IHttpActionResult result = _controller.Get("LMS/tst-azhang1/HS/Java/-K2hf2hPR4FZNk6ipEyn", time);
            Assert.IsInstanceOfType(result, typeof (OkNegotiatedContentResult<LMSresult>));

            IHttpActionResult result2 = _controller.Get("LMS/tst-azhang1/HS/Java/-K2hf2hPR4FZNk6ipEyn", 0);
            Assert.IsInstanceOfType(result2, typeof (OkNegotiatedContentResult<LMSresult>));

            IHttpActionResult result3 = _controller.Get("LM", 0);
            Assert.IsInstanceOfType(result3, typeof (OkNegotiatedContentResult<LMSresult>));
        }

        [TestMethod]
        public void GetChat_Test()
        {
            var time = (long) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;

            IHttpActionResult result = _controller.GetChat("-K0D7IEKYJeQpB01dMgF", time);
            Assert.IsInstanceOfType(result, typeof (OkNegotiatedContentResult<LMSChatresult>));

            IHttpActionResult result2 = _controller.GetChat("-K0D7IEKYJeQpB01dMgF", 0);
            Assert.IsInstanceOfType(result2, typeof (OkNegotiatedContentResult<LMSChatresult>));

            IHttpActionResult result3 = _controller.GetChat("", time);
            Assert.IsInstanceOfType(result3, typeof (OkNegotiatedContentResult<LMSChatresult>));
        }
    }
}