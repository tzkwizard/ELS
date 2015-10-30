using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using LMS.Common.Models.ELS;
using MessageHandleApi.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessageHandleApi.Test.Controllers
{
    [TestClass]
    public class ElasticSearchControllerTest
    {

        [TestMethod]
        public async Task GetSampledata_Test()
        {
            var testProducts = TestData.GetTestProducts();
            var controller = new ElasticSearchController();

            foreach (var item in testProducts)
            {
                IHttpActionResult result = await controller.GetSampledata(item);             
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<ELSresult>));
                IHttpActionResult result2 = await controller.GetSampledata(null);
                Assert.IsInstanceOfType(result2, typeof(BadRequestErrorMessageResult));
            }
           
        }
        [TestMethod]
        public async Task StringQuery_Test()
        {
            var testProducts = TestData.GetTestProducts();
            var controller = new ElasticSearchController();

            IHttpActionResult result = await controller.StringQuery(testProducts[0]);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<ELSresult>));
            IHttpActionResult result2 = await controller.StringQuery(null);
            Assert.IsInstanceOfType(result2, typeof(BadRequestErrorMessageResult));
        }
        [TestMethod]
        public async Task TermQuery_Test()
        {
            var testProducts = TestData.GetTestProducts();
            var controller = new ElasticSearchController();

            IHttpActionResult result = await controller.TermQuery(testProducts[0]);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<ELSresult>));
            IHttpActionResult result2 = await controller.TermQuery(null);
            Assert.IsInstanceOfType(result2, typeof(BadRequestErrorMessageResult));
        }
        [TestMethod]
        public async Task StringQueryWithBoolFilter_Test()
        {
            var testProducts = TestData.GetTestProducts();
            var controller = new ElasticSearchController();

            IHttpActionResult result = await controller.StringQueryWithBoolFilter(testProducts[0]);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<ELSresult>));
            IHttpActionResult result2 = await controller.StringQueryWithBoolFilter(null);
            Assert.IsInstanceOfType(result2, typeof(BadRequestErrorMessageResult));
        }
        [TestMethod]
        public async Task TermQueryWithBoolFilter_Test()
        {
            var testProducts = TestData.GetTestProducts();
            var controller = new ElasticSearchController();

            IHttpActionResult result = await controller.TermQueryWithBoolFilter(testProducts[0]);
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<ELSresult>));
            IHttpActionResult result2 = await controller.TermQueryWithBoolFilter(null);
            Assert.IsInstanceOfType(result2, typeof(BadRequestErrorMessageResult));
        }


    }
}
