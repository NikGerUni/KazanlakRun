using KazanlakRun.Web.Areas.Admin.Controllers;
using KazanlakRun.Web.Areas.Admin.Models;
using KazanlakRun.Web.Areas.Admin.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KazanlakRun.Web.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class AdminVolunteerControllerTests
    {
        private Mock<IVolunteerServiceAdmin> _mockService;
        private VolunteerController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockService = new Mock<IVolunteerServiceAdmin>();
            _controller = new VolunteerController(_mockService.Object);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [TearDown]
        public void TearDown()
        {
            if (_controller is IDisposable d) d.Dispose();
            if (_mockService is IDisposable e) e.Dispose();
        }

        [Test]
        public async Task Index_ReturnsViewWithList()
        {
            var list = new List<VolunteerListItem>
            {
                new VolunteerListItem { Id = 1, Names = "A" },
                new VolunteerListItem { Id = 2, Names = "B" }
            };
            _mockService
              .Setup(s => s.GetAllVolunteersAsync())
              .ReturnsAsync(list);

            var result = await _controller.Index() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreSame(list, result!.Model);
        }
        [Test]
        public async Task Create_Get_ReturnsViewWithVm()
        {
            var vm = new VolunteerViewModel { Id = 0 };
            _mockService
                .Setup(s => s.GetForCreateAsync())
                .ReturnsAsync(vm);

            var result = await _controller.Create() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreSame(vm, result!.Model);
        }

        [Test]
        public async Task Create_Post_InvalidModel_ReturnsViewWithSameModel()
        {
            var input = new VolunteerViewModel();
            _controller.ModelState.AddModelError("x", "err");

            var result = await _controller.Create(input) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreSame(input, result!.Model);
            _mockService.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Create_Post_ValidModel_CallsServiceAndRedirects()
        {
            var input = new VolunteerViewModel { Id = 0 };

            var result = await _controller.Create(input) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(nameof(_controller.Index), result!.ActionName);
            _mockService.Verify(s => s.CreateAsync(input), Times.Once);
        }

        [Test]
        public async Task Edit_Get_ReturnsViewWithVm()
        {
            var vm = new VolunteerViewModel { Id = 5 };
            _mockService
                .Setup(s => s.GetForEditAsync(5))
                .ReturnsAsync(vm);

            var result = await _controller.Edit(5) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreSame(vm, result!.Model);
        }

        [Test]
        public async Task Edit_Post_InvalidModel_ReturnsViewWithSameModel()
        {
            var input = new VolunteerViewModel { Id = 5 };
            _controller.ModelState.AddModelError("x", "err");

            var result = await _controller.Edit(input) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreSame(input, result!.Model);
            _mockService.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Edit_Post_ValidModel_CallsServiceAndRedirects()
        {
            var input = new VolunteerViewModel { Id = 5 };

            var result = await _controller.Edit(input) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(nameof(_controller.Index), result!.ActionName);
            _mockService.Verify(s => s.UpdateAsync(input), Times.Once);
        }

        [Test]
        public async Task Delete_Get_ReturnsViewWithVm()
        {
            var vm = new VolunteerViewModel { Id = 7 };
            _mockService
                .Setup(s => s.GetForEditAsync(7))
                .ReturnsAsync(vm);

            var result = await _controller.Delete(7) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreSame(vm, result!.Model);
        }

        [Test]
        public async Task DeleteConfirmed_Post_CallsServiceAndRedirects()
        {
            var result = await _controller.DeleteConfirmed(7) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(nameof(_controller.Index), result!.ActionName);
            _mockService.Verify(s => s.DeleteAsync(7), Times.Once);
        }
    }
}
