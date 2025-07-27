using KazanlakRun.Web.Areas.Admin.Controllers;
using KazanlakRun.Web.Areas.Admin.Models;
using KazanlakRun.Web.Areas.Admin.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace KazanlakRun.Web.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class AdminRoleControllerTests
    {
        private Mock<IRoleService> _service;
        private Mock<ILogger<RoleController>> _logger;
        private RoleController _controller;

        [SetUp]
        public void SetUp()
        {
            _service = new Mock<IRoleService>();
            _logger = new Mock<ILogger<RoleController>>();
            _controller = new RoleController(_service.Object, _logger.Object);

            // Точно това е ключовото: TempData не бива да е null
            var httpContext = new DefaultHttpContext();
            var tempProvider = new Mock<ITempDataProvider>().Object;
            _controller.TempData = new TempDataDictionary(httpContext, tempProvider);
        }

        [TearDown]
        public void TearDown()
        {
            if (_controller is IDisposable disp)
                disp.Dispose();
        }

        [Test]
        public async Task Index_ReturnsView_WithModelFromService()
        {
            // arrange
            var data = new List<RoleViewModel> { new() { Id = 1, Name = "X" } };
            _service.Setup(s => s.GetAllAsync()).ReturnsAsync(data);

            // act
            var result = await _controller.Index() as ViewResult;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(data, result.Model);
        }

        [Test]
        public async Task RowTemplate_ReturnsPartial_WithEmptyModelAndPlaceholder()
        {
            // act
            var result = _controller.RowTemplate() as PartialViewResult;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual("_RoleRow", result.ViewName);
            Assert.IsInstanceOf<RoleViewModel>(result.Model);
            Assert.AreEqual("__index__", result.ViewData["idx"]);
        }

        [Test]
        public async Task SaveAll_InvalidModelState_ReturnsIndexView()
        {
            // arrange
            _controller.ModelState.AddModelError("Name", "Required");

            // act
            var result = await _controller.SaveAll(new List<RoleViewModel>())
                               as ViewResult;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ViewName);
        }

        [Test]
        public async Task SaveAll_ServiceThrowsError_ReturnsViewWithError()
        {
            // arrange
            var roles = new List<RoleViewModel>();
            _service
              .Setup(s => s.SaveAllAsync(roles))
              .ThrowsAsync(new InvalidOperationException());

            // act
            var result = await _controller.SaveAll(roles) as ViewResult;

            // assert
            Assert.IsNotNull(result);
            Assert.IsFalse(_controller.ModelState.IsValid);

            var err = _controller.ModelState[string.Empty].Errors[0].ErrorMessage;
            Assert.That(err, Does.Contain("error").IgnoreCase);
        }

        [Test]
        public async Task SaveAll_Valid_RedirectsToIndex()
        {
            // arrange
            var roles = new List<RoleViewModel>();
            _service
              .Setup(s => s.SaveAllAsync(It.IsAny<List<RoleViewModel>>()))
              .Returns(Task.CompletedTask);

            // act
            var result = await _controller.SaveAll(roles);

            // assert
            Assert.IsInstanceOf<RedirectToActionResult>(result);
            var redirect = (RedirectToActionResult)result;
            Assert.AreEqual("Index", redirect.ActionName);
        }


    }
}
