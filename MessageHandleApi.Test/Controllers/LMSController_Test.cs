using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Results;
using LMS.Common.Models;
using LMS.Common.Models.Api;
using LMS.Common.Service;
using MessageHandleApi.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace MessageHandleApi.Test.Controllers
{
    [TestClass]
    public class LMSControllerTest
    {
        private static LMSController _controller;
        private static IDbService _iDbService;
        public LMSControllerTest()
        {
            IQueueService iQueueService = new QueueService();
            //IDbService iDbService = new DbService();
            _iDbService = Substitute.For<IDbService>();
            IAzureStorageService iAzureStorageService = new AzureStorageService();
            _controller = new LMSController(iQueueService, _iDbService, iAzureStorageService);
        }

        [TestMethod]
        public void Get_Test()
        {
            var time = (long) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;

            _iDbService.GetMoreList("LMS/tst-azhang1/HS/Java/-K2hf2hPR4FZNk6ipEyn", time).Returns(new LMSresult
            {
                time = 12,
                list = new List<PostMessage>(),
                moreData = true
            });
    

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