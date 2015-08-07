using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using MessageHandleApi.Controllers;
using MessageHandleApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessageHandleApi.Test.Controllers
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ElasticMappingControllerTest
    {
        [TestMethod]
        public async Task AutoFill_Test()
        {
            var testProducts = TestData.GetTestProducts();
            var controller = new ElasticMappingController();

            foreach (var item in testProducts)
            {
                IHttpActionResult result = await controller.AutoFill(item);
                Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<ELSresult>));
                IHttpActionResult result2 = await controller.AutoFill(null);
                Assert.IsInstanceOfType(result2, typeof(BadRequestErrorMessageResult));
            }

        }
        [TestMethod]
        public async Task Map_Test()
        {
            var testProducts = TestData.GetTestProducts();
            var controller = new ElasticMappingController();

            IHttpActionResult result = await controller.Map();
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<ELSmap>));
        }
        [TestMethod]
        public void CheckId_Test()
        {
            var controller = new ElasticMappingController();

            IHttpActionResult result = controller.CheckId("aotuo", "123456");
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<string>));

            IHttpActionResult result2 = controller.CheckId("aotuo", "");
            Assert.IsInstanceOfType(result2, typeof(BadRequestErrorMessageResult));


            IHttpActionResult result3 = controller.CheckId("", "123456");
            Assert.IsInstanceOfType(result3, typeof(BadRequestErrorMessageResult));

            IHttpActionResult result4 = controller.CheckId(null,null);
            Assert.IsInstanceOfType(result4, typeof(BadRequestErrorMessageResult));
        }




    }
}
