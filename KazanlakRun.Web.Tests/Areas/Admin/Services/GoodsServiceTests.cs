using KazanlakRun.Data;
using KazanlakRun.Data.Models;
using KazanlakRun.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using KazanlakRun.Web.Services.IServices;
using NUnit.Framework;

namespace KazanlakRun.Tests.Services
{
    [TestFixture]
    public class GoodsServiceTests
    {
        private ApplicationDbContext _context;
        private GoodsService _service;
        private Mock<ICacheService> _cacheServiceMock;
        private Mock<ILogger<GoodsService>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // изолирана БД за всеки тест
                .Options;

            _context = new ApplicationDbContext(options);
            _loggerMock = new Mock<ILogger<GoodsService>>();
            _cacheServiceMock = new Mock<ICacheService>();
            _service = new GoodsService(_context, _loggerMock.Object, _cacheServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task CreateAsync_ShouldAddNewGood()
        {
            var good = new Good { Name = "Water", Measure = "L", Quantity = 100, QuantityOneRunner = 0.5 };

            var result = await _service.CreateAsync(good);

            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(_context.Goods.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnGood_WhenExists()
        {
            var good = new Good { Name = "Salt", Measure = "kg", Quantity = 2, QuantityOneRunner = 0.1 };
            _context.Goods.Add(good);
            await _context.SaveChangesAsync();

            var result = await _service.GetByIdAsync(good.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Salt"));
        }

        [Test]
        public async Task UpdateAsync_ShouldModifyExistingGood()
        {
            var good = new Good { Name = "Cups", Measure = "pcs", Quantity = 200, QuantityOneRunner = 1 };
            _context.Goods.Add(good);
            await _context.SaveChangesAsync();

            good.Name = "Paper Cups";
            good.Quantity = 180;

            var result = await _service.UpdateAsync(good);
            var updated = await _context.Goods.FindAsync(good.Id);

            Assert.That(result, Is.True);
            Assert.That(updated!.Name, Is.EqualTo("Paper Cups"));
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveGood()
        {
            var good = new Good { Name = "Bread", Measure = "pcs", Quantity = 20, QuantityOneRunner = 1 };
            _context.Goods.Add(good);
            await _context.SaveChangesAsync();

            var result = await _service.DeleteAsync(good.Id);

            Assert.That(result, Is.True);
            Assert.That(await _context.Goods.FindAsync(good.Id), Is.Null);
        }

        [Test]
        public async Task SaveBatchAsync_ShouldAddUpdateAndDeleteGoods()
        {
            // Setup: 1 съществуващ запис
            var existing = new Good { Name = "Banana", Measure = "pcs", Quantity = 100, QuantityOneRunner = 1 };
            _context.Goods.Add(existing);
            await _context.SaveChangesAsync();

            var batch = new List<Good>
            {
                new Good { Id = existing.Id, Name = "Bananas", Measure = "pcs", Quantity = 150, QuantityOneRunner = 1 },
                new Good { Id = 0, Name = "Apples", Measure = "kg", Quantity = 30, QuantityOneRunner = 0.3 }
            };

            var result = await _service.SaveBatchAsync(batch);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(g => g.Name == "Apples"), Is.True);
            Assert.That(result.Any(g => g.Name == "Bananas"), Is.True);
        }
    }
}

