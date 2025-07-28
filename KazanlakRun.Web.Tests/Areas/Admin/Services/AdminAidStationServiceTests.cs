
using KazanlakRun.Data.Models;
using KazanlakRun.Web.Areas.Admin.Models;
using KazanlakRun.Web.Areas.Admin.Services;
using KazanlakRun.Web.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace KazanlakRun.Web.Tests.Areas.Admin.Services
{
    [TestFixture]
    public class AdminAidStationServiceTests
    {
        private DbContextOptions<ApplicationDbContext> _options = null!;
        private ApplicationDbContext _db = null!;
        private AidStationService _svc = null!;
        private Mock<ICacheService> _cacheServiceMock;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _db = new ApplicationDbContext(options);
            _cacheServiceMock = new Mock<ICacheService>();

            _svc = new AidStationService(_db, _cacheServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

        [Test]
        public async Task GetAllAsync_ReturnsStationsWithDistancesAndVolunteers()
        {
            var dist = new Distance { Id = 1, Distans = "5K", RegRunners = 10 };
            var role = new Role { Id = 1, Name = "Leader" };
            var station = new AidStation { Id = 1, Name = "StationX", ShortName = "SX" };
            var volunteer = new Volunteer
            {
                Id = 1,
                Names = "Alice",
                Email = "a@x.com",
                Phone = "555",
                AidStationId = station.Id,
                VolunteerRoles = new List<VolunteerRole>()
            };
            var vr = new VolunteerRole { Volunteer = volunteer, Role = role };
            var ad = new AidStationDistance { AidStation = station, Distance = dist };

            _db.Distances.Add(dist);
            _db.Roles.Add(role);
            _db.AidStations.Add(station);
            _db.Volunteers.Add(volunteer);
            _db.VolunteerRoles.Add(vr);
            _db.AidStationDistances.Add(ad);
            await _db.SaveChangesAsync();
            _db.ChangeTracker.Clear();

            var list = await _svc.GetAllAsync();

            Assert.AreEqual(1, list.Count);
            var item = list.Single();
            Assert.AreEqual(1, item.Id);
            Assert.AreEqual("StationX", item.Name);
            CollectionAssert.AreEqual(
                new[] { "5K" },
                item.DistanceNames
            );
            CollectionAssert.AreEqual(
                new[] { "Alice – Leader" },
                item.VolunteerDescriptions
            );
        }

        [Test]
        public async Task GetForCreateAsync_ReturnsViewModelWithSelectLists()
        {
            var d1 = new Distance { Id = 2, Distans = "10K" };
            var vol = new Volunteer
            {
                Id = 1,
                Names = "Bob",
                Email = "bob@example.com",
                Phone = "555-0000",
                VolunteerRoles = new List<VolunteerRole>()
            };
            _db.Distances.Add(d1);
            _db.Volunteers.Add(vol);
            await _db.SaveChangesAsync();
            _db.ChangeTracker.Clear();

            var vm = await _svc.GetForCreateAsync();

            var distances = vm.AllDistances.ToList();
            var volunteers = vm.AllVolunteers.ToList();

            Assert.That(distances, Has.Count.EqualTo(1));
            Assert.AreEqual("10K", distances[0].Text);
            Assert.AreEqual("2", distances[0].Value);
            Assert.IsFalse(distances[0].Selected);

            Assert.That(volunteers, Has.Count.EqualTo(1));
            StringAssert.StartsWith("Bob", volunteers[0].Text);
            Assert.AreEqual("1", volunteers[0].Value);
            Assert.IsFalse(volunteers[0].Selected);
        }

        [Test]
        public async Task GetForEditAsync_Throws_WhenNotFound()
        {
            Assert.ThrowsAsync<KeyNotFoundException>(() => _svc.GetForEditAsync(999));
        }

        [Test]
        public async Task GetForEditAsync_ReturnsViewModelWithSelections()
        {
            var r = new Role { Id = 3, Name = "Support" };
            var d = new Distance { Id = 4, Distans = "15K" };
            var station = new AidStation { Id = 5, Name = "Y", ShortName = "Y" };
            var vol = new Volunteer
            {
                Id = 1,
                Names = "Carol",
                Email = "carol@example.com",
                Phone = "555-0101",
                VolunteerRoles = new List<VolunteerRole>()
            };

            station.AidStationDistances.Add(new AidStationDistance { DistanceId = d.Id });
            vol.AidStationId = station.Id;
            var vr = new VolunteerRole { VolunteerId = vol.Id, RoleId = r.Id };

            _db.Roles.Add(r);
            _db.Distances.Add(d);
            _db.AidStations.Add(station);
            _db.Volunteers.Add(vol);
            _db.VolunteerRoles.Add(vr);
            await _db.SaveChangesAsync();
            _db.ChangeTracker.Clear();

            var vm = await _svc.GetForEditAsync(station.Id);

            Assert.AreEqual(station.Id, vm.Id);
            Assert.AreEqual("Y", vm.Name);

            Assert.That(vm.AllDistances.Select(x => x.Value), Contains.Item("4"));
            Assert.IsTrue(vm.AllDistances.Single(x => x.Value == "4").Selected);

            Assert.That(vm.AllVolunteers.Select(x => x.Value), Contains.Item("1"));
            Assert.IsTrue(vm.AllVolunteers.Single(x => x.Value == "1").Selected);
        }


        public async Task CreateAsync(AidStationViewModel model)
        {
            var entity = new AidStation
            {
                Name = model.Name,
                ShortName = model.Name
            };
            foreach (var did in model.SelectedDistanceIds)
                entity.AidStationDistances
                      .Add(new AidStationDistance { DistanceId = did });

            _db.AidStations.Add(entity);
            await _db.SaveChangesAsync();

            entity.ShortName = model.Name + entity.Id;
            await _db.SaveChangesAsync();

            if (model.SelectedVolunteerIds.Any())
            {
                var vols = await _db.Volunteers
                    .Where(v => model.SelectedVolunteerIds.Contains(v.Id))
                    .ToListAsync();
                vols.ForEach(v => v.AidStationId = entity.Id);
                await _db.SaveChangesAsync();
            }
        }

        [Test]
        public async Task UpdateAsync_Throws_WhenNotFound()
        {
            var model = new AidStationViewModel { Id = 123 };
            Assert.ThrowsAsync<KeyNotFoundException>(() => _svc.UpdateAsync(model));
        }

        [Test]
        public async Task UpdateAsync_UpdatesNameDistancesAndVolunteerAssignments()
        {
            var dOld = new Distance { Id = 7, Distans = "OldD" };
            var dNew = new Distance { Id = 8, Distans = "NewD" };

            var volOld = new Volunteer
            {
                Id = 1,
                Names = "OldV",
                Email = "old@x.com",
                Phone = "111-1111",
                VolunteerRoles = new List<VolunteerRole>()
            };
            var volNew = new Volunteer
            {
                Id = 2,
                Names = "NewV",
                Email = "new@x.com",
                Phone = "222-2222",
                VolunteerRoles = new List<VolunteerRole>()
            };

            var station = new AidStation { Id = 9, Name = "Orig", ShortName = "O" };

            station.AidStationDistances.Add(new AidStationDistance { DistanceId = dOld.Id });
            volOld.AidStationId = station.Id;

            _db.Distances.AddRange(dOld, dNew);
            _db.Volunteers.AddRange(volOld, volNew);
            _db.AidStations.Add(station);
            await _db.SaveChangesAsync();
            _db.ChangeTracker.Clear();

            var model = new AidStationViewModel
            {
                Id = station.Id,
                Name = "Updated",
                SelectedDistanceIds = new[] { dNew.Id },
                SelectedVolunteerIds = new[] { volNew.Id }
            };

            await _svc.UpdateAsync(model);

            var st = await _db.AidStations
                .Include(a => a.AidStationDistances)
                .Include(a => a.Volunteers)
                .FirstAsync(a => a.Id == station.Id);

            Assert.AreEqual("Updated", st.Name);
            CollectionAssert.AreEquivalent(
                new[] { dNew.Id },
                st.AidStationDistances.Select(ad => ad.DistanceId)
            );
            Assert.IsFalse(st.Volunteers.Any(v => v.Id == volOld.Id));
            Assert.IsTrue(st.Volunteers.Any(v => v.Id == volNew.Id));
        }

        [Test]
        public async Task DeleteAsync_RemovesStation()
        {
            var station = new AidStation { Id = 10, Name = "Del", ShortName = "D" };
            _db.AidStations.Add(station);
            await _db.SaveChangesAsync();

            await _svc.DeleteAsync(station.Id);

            bool exists = await _db.AidStations.AnyAsync(a => a.Id == station.Id);
            Assert.IsFalse(exists);
        }

        [Test]
        public void DeleteAsync_Throws_WhenNotFound()
        {
            Assert.ThrowsAsync<KeyNotFoundException>(() => _svc.DeleteAsync(9999));
        }
    }
}
