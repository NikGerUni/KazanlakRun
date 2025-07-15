using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KazanlakRun.Data;
using KazanlakRun.Data.Models;
using KazanlakRun.Web.Areas.Admin.Services;
using KazanlakRun.Web.Areas.Admin.Services.IServices;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace KazanlakRun.Web.Tests.Areas.Admin.Services
{
    [TestFixture]
    public class AdminDistanceServiceTests
    {
        private DbContextOptions<ApplicationDbContext> _options = null!;
        private ApplicationDbContext _db = null!;
        private DistanceService _svc = null!;

        [SetUp]
        public void SetUp()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _db = new ApplicationDbContext(_options);
            _svc = new DistanceService(_db);
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

        [Test]
        public async Task GetAllAsync_ReturnsAllDistances()
        {
            // Arrange
            var items = new[]
            {
                new Distance { Id = 1, Distans = "5K",  RegRunners = 10 },
                new Distance { Id = 2, Distans = "10K", RegRunners = 20 },
            };
            await _db.Distances.AddRangeAsync(items);
            await _db.SaveChangesAsync();

            // Act
            var result = (await _svc.GetAllAsync()).ToList();

            // Assert
            Assert.AreEqual(2, result.Count);
            CollectionAssert.AreEquivalent(
                items.Select(x => x.Id),
                result.Select(x => x.Id)
            );
        }

        [Test]
        public async Task GetByIdAsync_ReturnsEntity_WhenExists()
        {
            // Arrange
            var d = new Distance { Id = 5, Distans = "5K", RegRunners = 7 };
            await _db.Distances.AddAsync(d);
            await _db.SaveChangesAsync();

            // Act
            var result = await _svc.GetByIdAsync(5);

            // Assert
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
            // Arrange
            var d = new Distance { Id = 10, Distans = "X", RegRunners = 1 };
            await _db.Distances.AddAsync(d);
            await _db.SaveChangesAsync();

            // Act
            d.RegRunners = 99;
            await _svc.UpdateAsync(d);

            // Assert
            var fromDb = await _db.Distances.FindAsync(10);
            Assert.IsNotNull(fromDb);
            Assert.AreEqual(99, fromDb!.RegRunners);
        }

        [Test]
        public async Task UpdateMultipleAsync_UpdatesOnlyExistingEntities()
        {
            // Arrange: seed two distances, one extra in the list
            var seed = new[]
            {
                new Distance { Id = 1, Distans = "A", RegRunners = 1 },
                new Distance { Id = 2, Distans = "B", RegRunners = 2 }
            };
            await _db.Distances.AddRangeAsync(seed);
            await _db.SaveChangesAsync();
            _db.ChangeTracker.Clear();

            // prepare update list: modify both and include a non-existent Id=99
            var updates = new List<Distance>
            {
                new Distance { Id = 1, RegRunners = 11 },
                new Distance { Id = 2, RegRunners = 22 },
                new Distance { Id = 99, RegRunners = 99 }
            };

            // Act
            await _svc.UpdateMultipleAsync(updates);

            // Assert
            var all = await _db.Distances.AsNoTracking().ToListAsync();
            Assert.AreEqual(2, all.Count);
            Assert.AreEqual(11, all.Single(x => x.Id == 1).RegRunners);
            Assert.AreEqual(22, all.Single(x => x.Id == 2).RegRunners);
        }

        [Test]
        public async Task UpdateMultipleAsync_DoesNothing_WhenListEmpty()
        {
            // Arrange: seed a distance
            var d = new Distance { Id = 7, Distans = "7K", RegRunners = 7 };
            await _db.Distances.AddAsync(d);
            await _db.SaveChangesAsync();

            // Act
            await _svc.UpdateMultipleAsync(new List<Distance>());

            // Assert: original unchanged
            var fromDb = await _db.Distances.FindAsync(7);
            Assert.IsNotNull(fromDb);
            Assert.AreEqual(7, fromDb!.RegRunners);
        }
    }
}

