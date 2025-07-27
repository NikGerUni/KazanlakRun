using KazanlakRun.Web.Areas.Admin.Controllers;
using KazanlakRun.Web.Areas.Admin.Models;
using KazanlakRun.Web.Areas.Admin.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace KazanlakRun.Web.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class AdminDistanceControllerTests
    {
        private Mock<IDistanceEditDtoService> _mockSvc;
        private DistanceController _controller;
        private DefaultHttpContext _httpContext;

        [SetUp]
        public void SetUp()
        {
            _mockSvc = new Mock<IDistanceEditDtoService>();
            _controller = new DistanceController(_mockSvc.Object);

            // За да има ControllerContext.HttpContext
            _httpContext = new DefaultHttpContext();
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
        public async Task EditAll_Get_ReturnsViewWithList()
        {
            // Arrange
            var distances = new[]
            {
                new DistanceEditDto { Id = 1, Distans = "5K",  RegRunners = 50  },
                new DistanceEditDto { Id = 2, Distans = "10K", RegRunners = 100 }
            };
            _mockSvc.Setup(s => s.GetAllAsync())
                    .ReturnsAsync(distances);

            // Act
            var result = await _controller.EditAll() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result!.Model as List<DistanceEditDto>;
            Assert.IsNotNull(model);
            CollectionAssert.AreEqual(distances, model);
        }

        [Test]
        public async Task EditAll_Post_InvalidModel_ReturnsViewWithSameModel()
        {
            // Arrange
            var list = new List<DistanceEditDto>
            {
                new() { Id = 1, Distans = "X", RegRunners = 10 }
            };
            _controller.ModelState.AddModelError("x", "err");

            // Act
            var result = await _controller.EditAll(list) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreSame(list, result!.Model);
            _mockSvc.VerifyNoOtherCalls();
        }

        [Test]
        public async Task EditAll_Post_ValidModel_CallsServiceAndRedirects()
        {
            // Arrange
            var list = new List<DistanceEditDto>
            {
                new() { Id = 1, Distans = "5K",  RegRunners = 50  },
                new() { Id = 2, Distans = "10K", RegRunners = 100 }
            };

            // Act
            var result = await _controller.EditAll(list) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result!.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
            Assert.AreEqual("Admin", result.RouteValues["area"]);
            _mockSvc.Verify(s => s.UpdateMultipleAsync(list), Times.Once);
        }

        [Test]
        public async Task Index_ReturnsViewWithList()
        {
            // Arrange
            var distances = new[] {
                new DistanceEditDto { Id = 1, Distans = "5K",  RegRunners = 50  },
                new DistanceEditDto { Id = 2, Distans = "10K", RegRunners = 100 }
            };
            _mockSvc.Setup(s => s.GetAllAsync())
                    .ReturnsAsync(distances);

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result!.Model as IEnumerable<DistanceEditDto>;
            Assert.IsNotNull(model);
            CollectionAssert.AreEqual(distances, model.ToList());
        }

        [Test]
        public async Task Edit_Get_NotFoundWhenNull()
        {
            // Arrange
            _mockSvc.Setup(s => s.GetByIdAsync(42))
                    .ReturnsAsync((DistanceEditDto?)null);

            // Act
            var result = await _controller.Edit(42);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Edit_Get_ReturnsViewWithModel()
        {
            // Arrange
            var dto = new DistanceEditDto { Id = 7, Distans = "7K", RegRunners = 70 };
            _mockSvc.Setup(s => s.GetByIdAsync(7))
                    .ReturnsAsync(dto);

            // Act
            var result = await _controller.Edit(7) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreSame(dto, result!.Model);
        }

        [Test]
        public async Task Edit_Post_InvalidModel_ReturnsViewWithSameModel()
        {
            // Arrange
            var m = new DistanceEditDto { Id = 3, Distans = "3K", RegRunners = 30 };
            _controller.ModelState.AddModelError("x", "err");

            // Act
            var result = await _controller.Edit(m) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreSame(m, result!.Model);
            _mockSvc.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Edit_Post_ValidModel_CallsServiceAndRedirects()
        {
            // Arrange
            var m = new DistanceEditDto { Id = 3, Distans = "3K", RegRunners = 30 };

            // Act
            var result = await _controller.Edit(m) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result!.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
            Assert.AreEqual("Admin", result.RouteValues["area"]);
            _mockSvc.Verify(s => s.UpdateAsync(m), Times.Once);
        }
    }
}
