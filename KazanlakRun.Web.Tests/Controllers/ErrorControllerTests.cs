using KazanlakRun.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KazanlakRun.Web.Tests.Controllers
{
    [TestFixture]
    public class ErrorControllerTests
    {
        private ErrorController _controller;
        private DefaultHttpContext _httpContext;

        [SetUp]
        public void SetUp()
        {
            _controller = new ErrorController();
            _httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContext
            };
        }

        [TearDown]
        public void TearDown()
        {
            if (_controller is IDisposable disposable)
                disposable.Dispose();
        }

        [Test]
        public void ExceptionHandler_Returns500View()
        {
            var result = _controller.ExceptionHandler() as ViewResult;

            Assert.IsNotNull(result, "Should be a ViewResult");
            Assert.AreEqual("500", result.ViewName, "The view name should be '500'");
        }

        [TestCase(404)]
        [TestCase(403)]
        [TestCase(501)]
        public void HttpStatusCodeHandler_SetsStatusCode_And_ReturnsCorrectView(int code)
        {
            var result = _controller.HttpStatusCodeHandler(code) as ViewResult;

            Assert.IsNotNull(result, "Should be a ViewResult");
            Assert.AreEqual(code.ToString(), result.ViewName, "ViewName should match the status code");

            Assert.AreEqual(code, _httpContext.Response.StatusCode, "Response.StatusCode should be set");

            Assert.IsTrue(result.ViewData.ContainsKey("ErrorCode"), "ViewData should contain 'ErrorCode'");
            Assert.AreEqual(code, result.ViewData["ErrorCode"], "ViewData[\"ErrorCode\"] should match the status code");
        }
    }
}
