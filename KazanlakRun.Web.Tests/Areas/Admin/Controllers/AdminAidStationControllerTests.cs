using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using KazanlakRun.Web.Areas.Admin.Controllers;
using KazanlakRun.Web.Areas.Admin.Models;
using KazanlakRun.Web.Areas.Admin.Services.IServices;

namespace KazanlakRun.Web.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class AdminAidStationControllerTests
    {
        private Mock<IAidStationService> _mockService;
        private AidStationController _controller;
        private DefaultHttpContext _httpContext;

        [SetUp]
        public void SetUp()
        {
            _mockService = new Mock<IAidStationService>();
            _controller = new AidStationController(_mockService.Object);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
        }

        [Test]
        public async Task Index_ReturnsViewWithList()
        {
            // Arrange
            var list = new List<AidStationListItem>  // ✅ Използвайте правилния тип
    {
        new AidStationListItem { /* init props if any */ },
        new AidStationListItem { /* … */ }
    };
            _mockService.Setup(s => s.GetAllAsync())
                        .ReturnsAsync(list);

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreSame(list, result!.Model);
        }
        [Test]
        public async Task Create_Get_ReturnsViewWithVm()
        {
            // Arrange
            var vm = new AidStationViewModel { /* … */ };
            _mockService.Setup(s => s.GetForCreateAsync())
                        .ReturnsAsync(vm);

            // Act
            var result = await _controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreSame(vm, result!.Model);
        }

        [Test]
        public async Task Create_Post_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var input = new AidStationViewModel();
            _controller.ModelState.AddModelError("x", "err");

            // Act
            var result = await _controller.Create(input) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreSame(input, result!.Model);
            _mockService.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Create_Post_ValidModel_CallsServiceAndRedirects()
        {
            // Arrange
            var input = new AidStationViewModel();

            // Act
            var result = await _controller.Create(input) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(nameof(_controller.Index), result!.ActionName);
            _mockService.Verify(s => s.CreateAsync(input), Times.Once);
        }

        [Test]
        public async Task Edit_Get_ReturnsViewWithVm()
        {
            // Arrange
            var vm = new AidStationViewModel { /* … */ };
            _mockService.Setup(s => s.GetForEditAsync(7))
                        .ReturnsAsync(vm);

            // Act
            var result = await _controller.Edit(7) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreSame(vm, result!.Model);
        }

        [Test]
        public async Task Edit_Post_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var input = new AidStationViewModel { /* … */ };
            _controller.ModelState.AddModelError("x", "err");

            // Act
            var result = await _controller.Edit(input) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreSame(input, result!.Model);
            _mockService.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Edit_Post_ValidModel_CallsServiceAndRedirects()
        {
            // Arrange
            var input = new AidStationViewModel();

            // Act
            var result = await _controller.Edit(input) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(nameof(_controller.Index), result!.ActionName);
            _mockService.Verify(s => s.UpdateAsync(input), Times.Once);
        }

        [Test]
        public async Task Delete_Get_ReturnsViewWithVm()
        {
            // Arrange
            var vm = new AidStationViewModel { /* … */ };
            _mockService.Setup(s => s.GetForEditAsync(5))
                        .ReturnsAsync(vm);

            // Act
            var result = await _controller.Delete(5) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreSame(vm, result!.Model);
        }

        [Test]
        public async Task DeleteConfirmed_Post_CallsServiceAndRedirects()
        {
            // Act
            var result = await _controller.DeleteConfirmed(5) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(nameof(_controller.Index), result!.ActionName);
            _mockService.Verify(s => s.DeleteAsync(5), Times.Once);
        }
    }
}
