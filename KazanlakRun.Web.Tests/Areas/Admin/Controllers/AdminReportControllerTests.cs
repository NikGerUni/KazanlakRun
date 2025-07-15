using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KazanlakRun.Data;
using KazanlakRun.Data.Models;
using KazanlakRun.Web.Areas.Admin.Controllers;
using KazanlakRun.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using NUnit.Framework;

namespace KazanlakRun.Web.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class ReportControllerTests
    {
        private DbContextOptions<ApplicationDbContext> _options = null!;
        private ApplicationDbContext _db = null!;
        private ReportController _controller = null!;

        [SetUp]
        public void SetUp()
        {
            // Използваме in‑memory database с уникално име за всяко изпълнение
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _db = new ApplicationDbContext(_options);
            _controller = new ReportController(_db);
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
            _controller.Dispose();
        }

        [Test]
        public async Task RunnersByAidStation_ReturnsCorrectModel()
        {
            // Arrange: две дистанции и една станция с връзки
            var d1 = new Distance { Id = 1, Distans = "5K", RegRunners = 10 };
            var d2 = new Distance { Id = 2, Distans = "10K", RegRunners = 5 };
            var station = new AidStation
            {
                Id = 1,
                Name = "StationA",
                ShortName = "STA"
            };
            var ad1 = new AidStationDistance { AidStation = station, Distance = d1 };
            var ad2 = new AidStationDistance { AidStation = station, Distance = d2 };

            _db.Distances.AddRange(d1, d2);
            _db.AidStations.Add(station);
            _db.AidStationDistances.AddRange(ad1, ad2);
            await _db.SaveChangesAsync();

            // Detach, за да избегнем двоен тракинг
            _db.ChangeTracker.Clear();

            // Act
            var result = await _controller.RunnersByAidStation() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result!.Model as List<AidStationRunnersReportViewModel>;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count);

            var vm = model[0];
            Assert.AreEqual("StationA", vm.AidStationName);
            Assert.AreEqual(2, vm.Distances.Count);
            CollectionAssert.AreEquivalent(
                new[] { ("5K", 10), ("10K", 5) },
                vm.Distances.Select(d => (d.DistanceName, d.RegRunners))
            );
        }


        [Test]
        public async Task GoodsByAidStation_ReturnsCorrectModel()
        {
            // Arrange: два артикула и една станция с дистанция
            var g1 = new Good { Id = 1, Name = "Water", Measure = "bottle", QuantityOneRunner = 2 };
            var g2 = new Good { Id = 2, Name = "Snack", Measure = "pack", QuantityOneRunner = 1 };
            var d1 = new Distance { Id = 3, Distans = "5K", RegRunners = 4 };
            var station = new AidStation
            {
                Id = 3,
                Name = "StationC",
                ShortName = "STC"
            };
            var ad = new AidStationDistance { AidStation = station, Distance = d1 };

            _db.Goods.AddRange(g1, g2);
            _db.Distances.Add(d1);
            _db.AidStations.Add(station);
            _db.AidStationDistances.Add(ad);
            await _db.SaveChangesAsync();
            _db.ChangeTracker.Clear();

            // Act
            var result = await _controller.GoodsByAidStation() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            var model = result!.Model as List<AidStationGoodsReportViewModel>;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count);

            var vm = model[0];
            Assert.AreEqual("StationC", vm.AidStationName);
            // totalRunners = 4
            Assert.AreEqual(4, vm.TotalRegisteredRunners);
            // Water: 4*2=8, Snack:4*1=4
            Assert.AreEqual(2, vm.Goods.Count);
            var reportMap = vm.Goods.ToDictionary(g => g.Name);
            Assert.AreEqual(8, reportMap["Water"].QuantityPerAidStation);
            Assert.AreEqual(4, reportMap["Snack"].QuantityPerAidStation);
        }
    }
}
