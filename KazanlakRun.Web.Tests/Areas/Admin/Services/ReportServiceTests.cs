using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using KazanlakRun.Data;
using KazanlakRun.Data.Models;
using KazanlakRun.Web.Areas.Admin.Controllers;
using KazanlakRun.Web.Areas.Admin.Models;
using KazanlakRun.Web.Areas.Admin.Services;
using KazanlakRun.Web.Areas.Admin.Services.IServices;

namespace KazanlakRun.Tests
{


    [TestFixture]
    public class ReportServiceTests
    {
        private ApplicationDbContext _context;
        private ReportService _service;

        [SetUp]

        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            // Seed data
            var station = new AidStation
            {
                Id = 1,
                Name = "S1",
                ShortName = "S1"    // <- добавяме това
            };
            var distance = new Distance
            {
                Id = 1,
                Distans = "10K",
                RegRunners = 3
            };
            var link = new AidStationDistance
            {
                AidStationId = 1,
                DistanceId = 1
            };
            var good = new Good
            {
                Id = 1,
                Name = "Banana",
                Measure = "pcs",
                QuantityOneRunner = 2
            };

            _context.AidStations.Add(station);
            _context.Distances.Add(distance);
            _context.AidStationDistances.Add(link);
            _context.Goods.Add(good);
            _context.SaveChanges();

            _service = new ReportService(_context, new NullLogger<ReportService>());
        }
        [TearDown]
        public void TearDown()
        {
            // Освобождаваме context-а
            _context.Dispose();
                  }
        [Test]
        public async Task GetGoodsByAidStationAsync_ComputesCorrectQuantities()
        {
            // Act
            var result = await _service.GetGoodsByAidStationAsync();

            // Assert
            Assert.AreEqual(1, result.Count);
            var report = result.First();
            Assert.AreEqual("S1", report.AidStationName);
            Assert.AreEqual(3, report.TotalRegisteredRunners);
            Assert.AreEqual(1, report.Goods.Count);
            Assert.AreEqual("Banana", report.Goods[0].Name);
            Assert.AreEqual("pcs", report.Goods[0].Measure);
            // 3 runners * 2 pcs each = 6
            Assert.AreEqual(6, report.Goods[0].QuantityPerAidStation);
        }

        [Test]
        public async Task GetGoodsForDeliveryAsync_ComputesNeededQuantities()
        {
            // Arrange
            // Add second station with same distance
            var station2 = new KazanlakRun.Data.Models.AidStation { Id = 2, Name = "S2", ShortName = "S2" };
            _context.AidStations.Add(station2);
            _context.AidStationDistances.Add(new KazanlakRun.Data.Models.AidStationDistance { AidStationId = 2, DistanceId = 1 });
            _context.SaveChanges();

            // Act
            var result = await _service.GetGoodsForDeliveryAsync();

            // Assert
            // Two stations x 3 runners x 2 pcs = 12
            var delivery = result.First(r => r.Name == "Banana");
            Assert.AreEqual(12m, delivery.NeededQuantity);
        }
    }
}

