using KazanlakRun.Data.Models;
using KazanlakRun.Web.Areas.Admin.Models;
using KazanlakRun.Web.Areas.Admin.Services;
using Microsoft.EntityFrameworkCore;
using KazanlakRun.Web.Services.IServices;
using Moq;

namespace KazanlakRun.Web.Tests.Areas.Admin.Services
{
    [TestFixture]
    public class AdminDistanceServiceTests
    {
        private DbContextOptions<ApplicationDbContext> _options = null!;
        private ApplicationDbContext _db = null!;
        private DistanceEditDtoService _svc = null!;
        private Mock<ICacheService> _cacheServiceMock;

        [SetUp]
        public void SetUp()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _cacheServiceMock = new Mock<ICacheService>();
            _db = new ApplicationDbContext(_options);
            _svc = new DistanceEditDtoService(_db, _cacheServiceMock.Object);

        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

        [Test]
        public async Task GetAllAsync_ReturnsAllDistances()
        {
            var items = new[]
            {
                new Distance { Id = 1, Distans = "5K",  RegRunners = 10 },
                new Distance { Id = 2, Distans = "10K", RegRunners = 20 },
            };
            await _db.Distances.AddRangeAsync(items);
            await _db.SaveChangesAsync();

            var result = (await _svc.GetAllAsync()).ToList();

            Assert.AreEqual(2, result.Count);
            CollectionAssert.AreEquivalent(
                items.Select(x => x.Id),
                result.Select(x => x.Id)
            );
            Assert.AreEqual(10, result.Single(r => r.Id == 1).RegRunners);
            Assert.AreEqual(20, result.Single(r => r.Id == 2).RegRunners);
        }

        [Test]
        public async Task GetByIdAsync_ReturnsDto_WhenExists()
        {
            var d = new Distance { Id = 5, Distans = "5K", RegRunners = 7 };
            await _db.Distances.AddAsync(d);
            await _db.SaveChangesAsync();

            var result = await _svc.GetByIdAsync(5);

            Assert.IsNotNull(result);
            Assert.AreEqual(5, result!.Id);
            Assert.AreEqual("5K", result.Distans);
            Assert.AreEqual(7, result.RegRunners);
        }

        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            var result = await _svc.GetByIdAsync(123);

            Assert.IsNull(result);
        }

        [Test]
        public async Task UpdateAsync_UpdatesSingleEntity()
        {
            var d = new Distance { Id = 10, Distans = "X", RegRunners = 1 };
            await _db.Distances.AddAsync(d);
            await _db.SaveChangesAsync();
            _db.ChangeTracker.Clear();

            var dto = new DistanceEditDto
            {
                Id = 10,
                Distans = d.Distans,
                RegRunners = 99
            };
            await _svc.UpdateAsync(dto);

            var fromDb = await _db.Distances.FindAsync(10);
            Assert.IsNotNull(fromDb);
            Assert.AreEqual(99, fromDb!.RegRunners);
        }

        [Test]
        public void UpdateMultipleAsync_Throws_WhenEntityNotFound()
        {
            var svc = new DistanceEditDtoService(_db, _cacheServiceMock.Object);
            var model = new List<DistanceEditDto>
    {
        new DistanceEditDto { Id = 99, RegRunners = 5, Distans = "Test" }
    };

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await svc.UpdateMultipleAsync(model));
        }

        [Test]
        public async Task UpdateMultipleAsync_DoesNothing_WhenListEmpty()
        {
            var d = new Distance { Id = 7, Distans = "7K", RegRunners = 7 };
            await _db.Distances.AddAsync(d);
            await _db.SaveChangesAsync();
            _db.ChangeTracker.Clear();

            await _svc.UpdateMultipleAsync(new List<DistanceEditDto>());

            var fromDb = await _db.Distances.FindAsync(7);
            Assert.IsNotNull(fromDb);
            Assert.AreEqual(7, fromDb!.RegRunners);
        }
    }
}
