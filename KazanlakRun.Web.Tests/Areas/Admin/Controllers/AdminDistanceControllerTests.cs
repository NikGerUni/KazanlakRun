using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KazanlakRun.Data.Models;
using KazanlakRun.Web.Areas.Admin.Controllers;
using KazanlakRun.Web.Areas.Admin.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace KazanlakRun.Web.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class AdminDistanceControllerTests
    {
        private Mock<IDistanceService> _mockSvc;
        private DistanceController _controller;
        private DefaultHttpContext _httpContext;

        [SetUp]
        public void SetUp()
        {
            _mockSvc = new Mock<IDistanceService>();
            _controller = new DistanceController(_mockSvc.Object);

            // (Не е строго необходимо, но за консистентност):
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
                new Distance { Id = 1, Distans = "5K", RegRunners = 50 },
                new Distance { Id = 2, Distans = "10K", RegRunners = 100 }
            };
            _mockSvc.Setup(s => s.GetAllAsync())
                    .ReturnsAsync(distances);

            // Act
            var result = await _controller.EditAll() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result!.Model as List<Distance>;
            Assert.IsNotNull(model);
            CollectionAssert.AreEqual(distances, model);
        }

        [Test]
        public async Task EditAll_Post_InvalidModel_ReturnsViewWithSameModel()
        {
            // Arrange
            var list = new List<Distance> { new() { Id = 1 } };
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
            var list = new List<Distance>
            {
                new() { Id = 1, Distans = "5K",  RegRunners = 50 },
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
                new Distance { Id = 1 }, new Distance { Id = 2 }
            };
            _mockSvc.Setup(s => s.GetAllAsync())
                    .ReturnsAsync(distances);

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result!.Model as IEnumerable<Distance>;
            Assert.IsNotNull(model);
            CollectionAssert.AreEqual(distances, model.ToList());
        }

        [Test]
        public async Task Edit_Get_NotFoundWhenNull()
        {
            // Arrange
            _mockSvc.Setup(s => s.GetByIdAsync(42))
                    .ReturnsAsync((Distance?)null);

            // Act
            var result = await _controller.Edit(42);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task Edit_Get_ReturnsViewWithModel()
        {
            // Arrange
            var dto = new Distance { Id = 7, Distans = "7K" };
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
            var m = new Distance { Id = 3 };
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
            var m = new Distance { Id = 3, Distans = "3K", RegRunners = 30 };

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
