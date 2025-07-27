using KazanlakRun.Data.Models;
using KazanlakRun.Web.Areas.Admin.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace KazanlakRun.Web.Tests.Areas.Admin.Services
{
    public class ReportServiceTests
    {
        private ApplicationDbContext _db;
        private ReportService _service;

        [SetUp]
        public void SetUp()
        {
            var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: $"TestDb_{TestContext.CurrentContext.Test.ID}")
               .Options;

            _db = new ApplicationDbContext(opts);
            _service = new ReportService(_db, NullLogger<ReportService>.Instance);
        }

        [TearDown]
        public void TearDown()
        {
            _db.Database.EnsureDeleted();
            _db.Dispose();
        }

        [Test]
        public async Task GetRunnersByAidStationAsync_ReturnsStationsWithDistances()
        {
            // Arrange
            var distance1 = new Distance { Distans = "10K", RegRunners = 5 };
            var distance2 = new Distance { Distans = "5K", RegRunners = 3 };

            _db.AidStations.Add(new AidStation
            {
                Name = "Station1",
                ShortName = "ST1",  // required
                AidStationDistances = new List<AidStationDistance>
                {
                    new() { Distance = distance1 },
                    new() { Distance = distance2 }
                }
            });
            await _db.SaveChangesAsync();

            // Act
            var result = await _service.GetRunnersByAidStationAsync();

            // Assert
            Assert.That(result, Has.Count.EqualTo(1));
            var station = result.Single();
            Assert.That(station.AidStationName, Is.EqualTo("Station1"));
            Assert.That(station.Distances.Select(d => d.DistanceName),
                        Is.EquivalentTo(new[] { "10K", "5K" }));
            Assert.That(station.Distances.Select(d => d.RegRunners),
                        Is.EquivalentTo(new[] { 5, 3 }));
        }

        [Test]
        public async Task GetVolunteersByAidStationAsync_ReturnsStationsWithVolunteers()
        {
            // Arrange
            var role = new Role { Name = "Helper" };
            var volunteer = new Volunteer
            {
                Names = "Alice",
                Email = "alice@example.com",
                Phone = "12345",
                VolunteerRoles = new List<VolunteerRole>
                {
                    new() { Role = role }
                }
            };

            _db.AidStations.Add(new AidStation
            {
                Name = "Station2",
                ShortName = "ST2",  // required
                Volunteers = new List<Volunteer> { volunteer }
            });
            await _db.SaveChangesAsync();

            // Act
            var result = await _service.GetVolunteersByAidStationAsync();

            // Assert
            Assert.That(result, Has.Count.EqualTo(1));
            var station = result.Single();
            Assert.That(station.AidStationName, Is.EqualTo("Station2"));
            var v = station.Volunteers.Single();
            Assert.That(v.Names, Is.EqualTo("Alice"));
            Assert.That(v.Email, Is.EqualTo("alice@example.com"));
            Assert.That(v.Phone, Is.EqualTo("12345"));
            Assert.That(v.Roles, Is.EquivalentTo(new[] { "Helper" }));
        }

        [Test]
        public async Task GetGoodsByAidStationAsync_ComputesQuantityPerAidStation()
        {
            // Arrange
            var distance = new Distance { Distans = "Marathon", RegRunners = 2 };
            _db.AidStations.Add(new AidStation
            {
                Name = "Station3",
                ShortName = "ST3",  // required
                AidStationDistances = new List<AidStationDistance>
                {
                    new() { Distance = distance }
                }
            });

            _db.Goods.Add(new Good
            {
                Name = "Water",
                Measure = "L",
                QuantityOneRunner = 3
            });

            await _db.SaveChangesAsync();

            // Act
            var result = await _service.GetGoodsByAidStationAsync();

            // Assert
            Assert.That(result, Has.Count.EqualTo(1));
            var station = result.Single();
            Assert.That(station.AidStationName, Is.EqualTo("Station3"));
            Assert.That(station.TotalRegisteredRunners, Is.EqualTo(2));

            var good = station.Goods.Single();
            Assert.That(good.Name, Is.EqualTo("Water"));
            Assert.That(good.Measure, Is.EqualTo("L"));
            Assert.That(good.QuantityPerAidStation, Is.EqualTo(2 * 3));
        }

        [Test]
        public async Task GetGoodsForDeliveryAsync_ComputesNeededAndAvailableQuantities()
        {
            // Arrange
            var dist1 = new Distance { Distans = "D1", RegRunners = 1 };
            var dist2 = new Distance { Distans = "D2", RegRunners = 2 };

            _db.AidStations.AddRange(new[]
            {
                new AidStation
                {
                    Name                = "S1",
                    ShortName           = "S1",  // required
                    AidStationDistances = new List<AidStationDistance>
                    {
                        new() { Distance = dist1 }
                    }
                },
                new AidStation
                {
                    Name                = "S2",
                    ShortName           = "S2",  // required
                    AidStationDistances = new List<AidStationDistance>
                    {
                        new() { Distance = dist2 }
                    }
                }
            });

            _db.Goods.Add(new Good
            {
                Name = "Snack",
                Measure = "pcs",
                QuantityOneRunner = 2,
                Quantity = 5
            });

            await _db.SaveChangesAsync();

            // Act
            var result = await _service.GetGoodsForDeliveryAsync();

            // Assert
            Assert.That(result, Has.Count.EqualTo(1));
            var report = result.Single();
            Assert.That(report.Name, Is.EqualTo("Snack"));
            // needed = (1 + 2) * 2 = 6
            Assert.That(report.NeededQuantity, Is.EqualTo(6m));
            Assert.That(report.Quantity, Is.EqualTo(5m));
            Assert.That(report.ForDelivery, Is.EqualTo(1m));
        }
    }
}
