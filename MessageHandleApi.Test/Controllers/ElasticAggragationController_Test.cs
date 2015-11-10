using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using LMS.Common.Models.ELS;
using MessageHandleApi.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;

namespace MessageHandleApi.Test.Controllers
{
    [TestClass]
    public class ElasticAggragationControllerTest
    {
        [TestMethod]
        public async Task TermAggragation_Test()
        {
            var testProducts = TestData.GetTestProducts();
            var controller = new ElasticAggragationController();

            foreach (var item in testProducts)
            {
                IHttpActionResult result = await controller.TermAggragation(item);
                Assert.IsInstanceOfType(result, typeof (OkNegotiatedContentResult<ELSresult>));
                IHttpActionResult result2 = await controller.TermAggragation(null);
                Assert.IsInstanceOfType(result2, typeof (BadRequestErrorMessageResult));
            }
        }

        [TestMethod]
        public async Task TermAggragationwithQuery_Test()
        {
            var testProducts = TestData.GetTestProducts();
            var controller = new ElasticAggragationController();

            foreach (var item in testProducts)
            {
                IHttpActionResult result = await controller.TermAggragationwithQuery(item);
                Assert.IsInstanceOfType(result, typeof (OkNegotiatedContentResult<ELSresult>));
                IHttpActionResult result2 = await controller.TermAggragationwithQuery(null);
                Assert.IsInstanceOfType(result2, typeof (BadRequestErrorMessageResult));
            }
        }

        [TestMethod]
        public async Task TermQueryAggragation_Test()
        {
            var testProducts = TestData.GetTestProducts();
            var controller = new ElasticAggragationController();

            foreach (var item in testProducts)
            {
                IHttpActionResult result = await controller.TermQueryAggragation(item);
                Assert.IsInstanceOfType(result, typeof (OkNegotiatedContentResult<ELSresult>));
                IHttpActionResult result2 = await controller.TermQueryAggragation(null);
                Assert.IsInstanceOfType(result2, typeof (BadRequestErrorMessageResult));
            }
        }

        [TestMethod]
        public async Task DashboardPieAggregation_Test()
        {
            var testProducts = TestData.GetTestProducts();
            var controller = new ElasticAggragationController();

            foreach (var item in testProducts)
            {
                var res = new Dictionary<int, IList<KeyItem>>();
                IHttpActionResult result = await controller.DashboardPieAggregation(item);
                Assert.IsInstanceOfType(result, typeof (OkNegotiatedContentResult<Dictionary<int, IList<KeyItem>>>));
                IHttpActionResult result2 = await controller.DashboardPieAggregation(null);
                Assert.IsInstanceOfType(result2, typeof (BadRequestErrorMessageResult));
            }
        }

        [TestMethod]
        public async Task DateHistogramAggregation_Test()
        {
            var testProducts = TestData.GetTestProducts();
            var controller = new ElasticAggragationController();

            foreach (var item in testProducts)
            {
                IHttpActionResult result = await controller.DateHistogramAggregation(item);
                Assert.IsInstanceOfType(result, typeof (OkNegotiatedContentResult<ELSresult>));
                IHttpActionResult result2 = await controller.DateHistogramAggregation(null);
                Assert.IsInstanceOfType(result2, typeof (BadRequestErrorMessageResult));
            }
        }

        [TestMethod]
        public async Task GeoDistanceAggragation_Test()
        {
            var testProducts = TestData.GetTestProducts();
            var controller = new ElasticAggragationController();

            foreach (var item in testProducts)
            {
                IHttpActionResult result = await controller.GeoDistanceAggragation(item);
                Assert.IsInstanceOfType(result, typeof (OkNegotiatedContentResult<ELSresult>));
                IHttpActionResult result2 = await controller.GeoDistanceAggragation(null);
                Assert.IsInstanceOfType(result2, typeof (BadRequestErrorMessageResult));
            }
        }
    }
}