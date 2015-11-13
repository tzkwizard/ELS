using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using System.Web.Http.Results;
using LMS.Common.Models;
using LMS.Common.Models.Api;
using LMS.Common.Service;
using LMS.Common.Service.Interface;
using MessageHandleApi.Controllers;
using NSubstitute;
using NUnit.Framework;

namespace MessageHandleApi.Test
{
    [TestFixture]
    public class LmsTest
    {
        private static LmsController _controller;
        private static ILmsDashboardService _iLmsDashboardService;
        public LmsTest()
        {
            IQueueService iQueueService = new QueueService();
            IDbService iDbService = new DbService();
            _iLmsDashboardService = Substitute.For<ILmsDashboardService>();
            
            IAzureStorageService iAzureStorageService = new AzureStorageService();
            IResolverService iResolverService=new ResolverService();
            _controller = new LmsController(iDbService, iAzureStorageService);
        }

        [Test]
        public void Get_Test()
        {
            var time = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;

            _iLmsDashboardService.GetMoreList("LMS/tst-azhang1/HS/Java/-K2hf2hPR4FZNk6ipEyn", time).Returns(new LMSresult
            {
                time = 12,
                list = new List<PostMessage>(),
                moreData = true
            });


            IHttpActionResult result = _controller.Get("LMS/tst-azhang1/HS/Java/-K2hf2hPR4FZNk6ipEyn", time);
            Assert.IsInstanceOf<OkNegotiatedContentResult<LMSresult>>(result);
            IHttpActionResult result2 = _controller.Get("LMS/tst-azhang1/HS/Java/-K2hf2hPR4FZNk6ipEyn", 0);
            Assert.IsInstanceOf<OkNegotiatedContentResult<LMSresult>>(result2);

            IHttpActionResult result3 = _controller.Get("LM", 0);
            Assert.IsInstanceOf<OkNegotiatedContentResult<LMSresult>>(result3);
        }

        [Test]
        public void GetChat_Test()
        {
              var time = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;

              IHttpActionResult result = _controller.GetChat("-K0D7IEKYJeQpB01dMgF", time);
              Assert.IsInstanceOf<OkNegotiatedContentResult<LMSChatresult>>(result);

              IHttpActionResult result2 = _controller.GetChat("-K0D7IEKYJeQpB01dMgF", 0);
              Assert.IsInstanceOf<OkNegotiatedContentResult<LMSChatresult>>(result2);

              IHttpActionResult result3 = _controller.GetChat("", time);
              Assert.IsInstanceOf<OkNegotiatedContentResult<LMSChatresult>>(result3);
        }

        [Test]
        public void Eventhub_Test()
        {
            ConfigurationManager.AppSettings["LMS1"] = "";
            ConfigurationManager.AppSettings["LMS2"] = "";
            ConfigurationManager.AppSettings["LMS3"] = "";
            LmsSupport.SendingRandomMessages(1);
            //LmsSupport.ReceivePartitionMessage();

            /*Assert.AreEqual("true",ConfigurationManager.AppSettings["LMS1"]);
            Assert.AreEqual("true",ConfigurationManager.AppSettings["LMS2"]);
            Assert.AreEqual("true",ConfigurationManager.AppSettings["LMS3"]);*/
        }


    }
}