using KazanlakRun.Web.Areas.User.Controllers;
using KazanlakRun.Web.Areas.User.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace KazanlakRun.Web.Tests.Areas.User.Controllers
{
    [TestFixture]
    public class UserHomeControllerTests
    {
        private Mock<IVolunteerService> _mockVolunteerService;
        private HomeController _controller;
        private DefaultHttpContext _httpContext;

        [SetUp]
        public void SetUp()
        {
            _mockVolunteerService = new Mock<IVolunteerService>();

            _controller = new HomeController(_mockVolunteerService.Object);

            _httpContext = new DefaultHttpContext();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user-123")
            }, "TestAuth"));
            _httpContext.User = user;

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
        public async Task Index_WhenVolunteerExists_PassesTrueToView()
        {
            _mockVolunteerService
                .Setup(s => s.ExistsAsync("user-123"))
                .ReturnsAsync(true);

            var actionResult = await _controller.Index();
            var viewResult = actionResult as ViewResult;

            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOf<bool>(viewResult!.Model);
            Assert.IsTrue((bool)viewResult.Model);

            _mockVolunteerService.Verify(s => s.ExistsAsync("user-123"), Times.Once);
        }

        [Test]
        public async Task Index_WhenVolunteerDoesNotExist_PassesFalseToView()
        {
            _mockVolunteerService
                .Setup(s => s.ExistsAsync("user-123"))
                .ReturnsAsync(false);

            var actionResult = await _controller.Index();
            var viewResult = actionResult as ViewResult;

            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOf<bool>(viewResult!.Model);
            Assert.IsFalse((bool)viewResult.Model);

            _mockVolunteerService.Verify(s => s.ExistsAsync("user-123"), Times.Once);
        }
    }
}
