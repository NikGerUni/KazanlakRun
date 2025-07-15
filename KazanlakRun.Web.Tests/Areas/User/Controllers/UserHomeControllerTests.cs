using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using KazanlakRun.Web.Areas.User.Controllers;
using KazanlakRun.Web.Areas.User.Services;

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
            // 1) Подготовка на mock service
            _mockVolunteerService = new Mock<IVolunteerService>();

            // 2) Инстанциране на контролера с DI
            _controller = new HomeController(_mockVolunteerService.Object);

            // 3) Създаваме fake HttpContext с user claim
            _httpContext = new DefaultHttpContext();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user-123")
            }, "TestAuth"));
            _httpContext.User = user;

            // 4) За да работи User.FindFirstValue в контролера
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
            // Arrange: ExistsAsync да върне true за "user-123"
            _mockVolunteerService
                .Setup(s => s.ExistsAsync("user-123"))
                .ReturnsAsync(true);

            // Act
            var actionResult = await _controller.Index();
            var viewResult = actionResult as ViewResult;

            // Assert: правилен тип резултат
            Assert.IsNotNull(viewResult);
            // Assert: моделът е bool:true
            Assert.IsInstanceOf<bool>(viewResult!.Model);
            Assert.IsTrue((bool)viewResult.Model);

            // Assert: сервизът е извикан веднъж с правилния userId
            _mockVolunteerService.Verify(s => s.ExistsAsync("user-123"), Times.Once);
        }

        [Test]
        public async Task Index_WhenVolunteerDoesNotExist_PassesFalseToView()
        {
            // Arrange: ExistsAsync да върне false
            _mockVolunteerService
                .Setup(s => s.ExistsAsync("user-123"))
                .ReturnsAsync(false);

            // Act
            var actionResult = await _controller.Index();
            var viewResult = actionResult as ViewResult;

            // Assert
            Assert.IsNotNull(viewResult);
            Assert.IsInstanceOf<bool>(viewResult!.Model);
            Assert.IsFalse((bool)viewResult.Model);

            _mockVolunteerService.Verify(s => s.ExistsAsync("user-123"), Times.Once);
        }
    }
}
