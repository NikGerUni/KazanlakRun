using KazanlakRun.Data.Models;
using KazanlakRun.Services.Contracts;
using KazanlakRun.Web.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace KazanlakRun.Tests.Controllers
{
    [TestFixture]
    public class GoodsApiControllerTests
    {
        private Mock<IGoodsService> _goodsServiceMock;
        private GoodsApiController _controller;

        [SetUp]
        public void Setup()
        {
            _goodsServiceMock = new Mock<IGoodsService>();
            var logger = new Mock<ILogger<GoodsApiController>>();
            _controller = new GoodsApiController(_goodsServiceMock.Object, logger.Object);
        }

        [Test]
        public async Task GetGoods_ShouldReturnOkWithList()
        {
            // Arrange
            var sampleGoods = new List<Good> {
                new Good { Id = 1, Name = "Water", Measure = "L", Quantity = 100, QuantityOneRunner = 0.5 }
            };
            _goodsServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(sampleGoods);

            // Act
            var result = await _controller.GetGoods();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var returnedGoods = okResult.Value as List<Good>;
            Assert.That(returnedGoods, Has.Count.EqualTo(1));
            Assert.That(returnedGoods![0].Name, Is.EqualTo("Water"));
        }

        [Test]
        public async Task GetGood_WithExistingId_ReturnsGood()
        {
            var good = new Good { Id = 10, Name = "Bread", Measure = "pcs", Quantity = 10, QuantityOneRunner = 1 };
            _goodsServiceMock.Setup(s => s.GetByIdAsync(10)).ReturnsAsync(good);

            var result = await _controller.GetGood(10);

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.That((okResult!.Value as Good)!.Name, Is.EqualTo("Bread"));
        }

        [Test]
        public async Task GetGood_WithInvalidId_ReturnsNotFound()
        {
            _goodsServiceMock.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((Good?)null);

            var result = await _controller.GetGood(99);

            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task CreateGood_ShouldReturnCreated()
        {
            var good = new Good { Name = "Apples", Measure = "kg", Quantity = 20, QuantityOneRunner = 0.5 };
            var created = new Good { Id = 5, Name = "Apples", Measure = "kg", Quantity = 20, QuantityOneRunner = 0.5};

            _goodsServiceMock.Setup(s => s.CreateAsync(good)).ReturnsAsync(created);

            var result = await _controller.CreateGood(good);

            var createdResult = result.Result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.That((createdResult!.Value as Good)!.Id, Is.EqualTo(5));
        }

        [Test]
        public async Task UpdateGood_WithMismatchedId_ReturnsBadRequest()
        {
            var good = new Good { Id = 2, Name = "Milk" };

            var result = await _controller.UpdateGood(5, good);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task UpdateGood_WithValidData_ReturnsNoContent()
        {
            var good = new Good { Id = 1, Name = "Juice", Measure = "L", Quantity = 5, QuantityOneRunner = 0.2 };
            _goodsServiceMock.Setup(s => s.UpdateAsync(good)).ReturnsAsync(true);

            var result = await _controller.UpdateGood(1, good);

            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task DeleteGood_WhenNotFound_ReturnsNotFound()
        {
            _goodsServiceMock.Setup(s => s.DeleteAsync(100)).ReturnsAsync(false);

            var result = await _controller.DeleteGood(100);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task SaveBatch_WithEmptyList_ReturnsBadRequest()
        {
            var result = await _controller.SaveBatch(new List<Good>());

            Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task SaveBatch_WithValidData_ReturnsOk()
        {
            var goods = new List<Good>
            {
                new Good { Id = 0, Name = "Bananas", Measure = "pcs", Quantity = 100, QuantityOneRunner = 1 }
            };

            _goodsServiceMock.Setup(s => s.SaveBatchAsync(goods)).ReturnsAsync(goods);

            var result = await _controller.SaveBatch(goods);

            var ok = result.Result as OkObjectResult;
            Assert.That(ok, Is.Not.Null);
            var returned = ok!.Value as List<Good>;
            Assert.That(returned, Has.Count.EqualTo(1));
            Assert.That(returned![0].Name, Is.EqualTo("Bananas"));
        }
    }
}
