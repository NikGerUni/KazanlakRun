using KazanlakRun.Web.Areas.User.Controllers;
using KazanlakRun.Web.Areas.User.Models;
using KazanlakRun.Web.Areas.User.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace KazanlakRun.Web.Tests.Areas.User.Controllers
{
    [TestFixture]
    public class VolunteerControllerTests
    {
        private Mock<IVolunteerService> _mockService;
        private VolunteerController _controller;
        private DefaultHttpContext _httpContext;

        [SetUp]
        public void SetUp()
        {
            _mockService = new Mock<IVolunteerService>();
            _controller = new VolunteerController(_mockService.Object);

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
            if (_controller is IDisposable disp)
                disp.Dispose();
        }

        [Test]
        public void Create_Get_ReturnsDefaultView()
        {
            var result = _controller.Create() as ViewResult;

            Assert.IsNotNull(result, "Should return ViewResult");
            Assert.IsTrue(string.IsNullOrEmpty(result!.ViewName), "Default view should have empty ViewName");
            Assert.IsNull(result.Model, "Model should be null by default");
        }

        [Test]
        public async Task Create_Post_InvalidModel_ReturnsViewWithSameModel()
        {
            var input = new VolunteerInputModel { /* you can set properties here if needed */ };
            _controller.ModelState.AddModelError("dummy", "error");

            var result = await _controller.Create(input) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreSame(input, result!.Model, "Should return the same model when ModelState is invalid");
            _mockService.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Create_Post_ValidModel_CallsServiceAndRedirectsHomeIndex()
        {
            var input = new VolunteerInputModel();

            var result = await _controller.Create(input) as RedirectToActionResult;

            Assert.IsNotNull(result, "Should return RedirectToActionResult");
            Assert.AreEqual("Index", result!.ActionName);
            Assert.AreEqual("Home", result.ControllerName);

            _mockService.Verify(s => s.CreateAsync("user-123", input), Times.Once);
        }

        [Test]
        public async Task Edit_Get_NoExistingVolunteer_RedirectsToCreate()
        {
            _mockService
                .Setup(s => s.GetByUserIdAsync("user-123"))
                .ReturnsAsync((VolunteerInputModel?)null);

            var result = await _controller.Edit() as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Create", result!.ActionName);
            Assert.IsNull(result.ControllerName, "ControllerName should be null when redirecting within same controller");
        }

        [Test]
        public async Task Edit_Get_ExistingVolunteer_ReturnsViewWithModel()
        {
            var existing = new VolunteerInputModel { /*...*/ };
            _mockService
                .Setup(s => s.GetByUserIdAsync("user-123"))
                .ReturnsAsync(existing);

            var result = await _controller.Edit() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreSame(existing, result!.Model);
        }

        [Test]
        public async Task Edit_Post_InvalidModel_ReturnsViewWithSameModel()
        {
            var input = new VolunteerInputModel();
            _controller.ModelState.AddModelError("x", "err");

            var result = await _controller.Edit(input) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreSame(input, result!.Model);
            _mockService.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Edit_Post_ValidModel_CallsServiceAndRedirectsHomeIndex()
        {
            var input = new VolunteerInputModel();

            var result = await _controller.Edit(input) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result!.ActionName);
            Assert.AreEqual("Home", result.ControllerName);

            _mockService.Verify(s => s.UpdateAsync("user-123", input), Times.Once);
        }

        [Test]
        public async Task Delete_Post_CallsServiceAndRedirectsHomeIndex()
        {
            var result = await _controller.Delete() as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result!.ActionName);
            Assert.AreEqual("Home", result.ControllerName);

            _mockService.Verify(s => s.DeleteAsync("user-123"), Times.Once);
        }
    }
}
