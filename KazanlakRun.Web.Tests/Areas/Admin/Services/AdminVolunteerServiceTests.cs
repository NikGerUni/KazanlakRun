using KazanlakRun.Data.Models;
using KazanlakRun.Web.Areas.Admin.Models;
using KazanlakRun.Web.Areas.Admin.Services;
using Microsoft.EntityFrameworkCore;
using KazanlakRun.Web.Services.IServices;
using Moq;

namespace KazanlakRun.Web.Tests.Areas.Admin.Services
{
    [TestFixture]
    public class AdminVolunteerServiceTests
    {
        private DbContextOptions<ApplicationDbContext> _options = null!;
        private ApplicationDbContext _db = null!;
        private VolunteerServiceAdmin _svc = null!;
        private Mock<ICacheService> _cacheServiceMock;

        [SetUp]
        public void SetUp()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _db = new ApplicationDbContext(_options);
            _cacheServiceMock = new Mock<ICacheService>();
            _svc = new VolunteerServiceAdmin(_db, _cacheServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

        [Test]
        public async Task GetAllRolesAsync_ReturnsMappedRoles()
        {
            var roles = new[]
            {
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "User" }
            };
            _db.Roles.AddRange(roles);
            await _db.SaveChangesAsync();
            _db.ChangeTracker.Clear();

            var result = await _svc.GetAllRolesAsync();

            Assert.AreEqual(2, result.Count);
            CollectionAssert.AreEquivalent(
                new[] { 1, 2 }, result.Select(r => r.Id)
            );
            CollectionAssert.AreEquivalent(
                new[] { "Admin", "User" }, result.Select(r => r.Name)
            );
        }

        [Test]
        public async Task GetAllVolunteersAsync_ReturnsListItemsWithRoles()
        {
            var role1 = new Role { Id = 1, Name = "Leader" };
            var role2 = new Role { Id = 2, Name = "Support" };
            var vol = new Volunteer
            {
                Id = 10,
                Names = "Alice",
                Email = "alice@x.com",
                Phone = "123",
                VolunteerRoles = new List<VolunteerRole>
                {
                    new VolunteerRole { Role = role1 },
                    new VolunteerRole { Role = role2 }
                }
            };
            _db.Roles.AddRange(role1, role2);
            _db.Volunteers.Add(vol);
            await _db.SaveChangesAsync();
            _db.ChangeTracker.Clear();

            var list = await _svc.GetAllVolunteersAsync();

            Assert.AreEqual(1, list.Count);
            var item = list.Single();
            Assert.AreEqual(10, item.Id);
            Assert.AreEqual("Alice", item.Names);
            Assert.AreEqual("alice@x.com", item.Email);
            Assert.AreEqual("123", item.Phone);
            CollectionAssert.AreEquivalent(
                new[] { "Leader", "Support" },
                item.RoleNames
            );
        }

        [Test]
        public async Task GetForCreateAsync_ReturnsViewModelWithSelectItems()
        {
            _db.Roles.Add(new Role { Id = 5, Name = "Gamma" });
            _db.Roles.Add(new Role { Id = 6, Name = "Delta" });
            await _db.SaveChangesAsync();

            var vm = await _svc.GetForCreateAsync();

            Assert.That(vm.AllRoles, Has.Count.EqualTo(2));
            var texts = vm.AllRoles.Select(i => i.Text).ToList();
            var vals = vm.AllRoles.Select(i => i.Value).ToList();
            CollectionAssert.AreEquivalent(new[] { "Gamma", "Delta" }, texts);
            CollectionAssert.AreEquivalent(new[] { "5", "6" }, vals);
            Assert.IsTrue(vm.AllRoles.All(i => i.Selected == false));
        }

        [Test]
        public async Task CreateAsync_AddsVolunteerAndRoles()
        {
            var model = new VolunteerViewModel
            {
                Names = "Bob",
                Email = "bob@x.com",
                Phone = "456",
                SelectedRoleIds = new[] { 1, 2 }
            };

            await _svc.CreateAsync(model);

            var v = await _db.Volunteers
                .Include(x => x.VolunteerRoles)
                .FirstAsync();
            Assert.AreEqual("Bob", v.Names);
            Assert.AreEqual("bob@x.com", v.Email);
            Assert.AreEqual("456", v.Phone);
            CollectionAssert.AreEquivalent(
                new[] { 1, 2 },
                v.VolunteerRoles.Select(vr => vr.RoleId)
            );
        }

        [Test]
        public async Task GetForEditAsync_ThrowsWhenNotFound()
        {
            Assert.ThrowsAsync<KeyNotFoundException>(
                () => _svc.GetForEditAsync(999)
            );
        }

        [Test]
        public async Task GetForEditAsync_ReturnsPopulatedViewModel()
        {
            var r1 = new Role { Id = 1, Name = "R1" };
            var r2 = new Role { Id = 2, Name = "R2" };
            _db.Roles.AddRange(r1, r2);
            var vol = new Volunteer
            {
                Id = 20,
                Names = "Cat",
                Email = "cat@x",
                Phone = "789",
                VolunteerRoles = new List<VolunteerRole>
                {
                    new VolunteerRole { RoleId = 1 }
                }
            };
            _db.Volunteers.Add(vol);
            await _db.SaveChangesAsync();
            _db.ChangeTracker.Clear();

            var vm = await _svc.GetForEditAsync(20);

            Assert.AreEqual(20, vm.Id);
            Assert.AreEqual("Cat", vm.Names);
            Assert.AreEqual("cat@x", vm.Email);
            Assert.AreEqual("789", vm.Phone);

            Assert.That(vm.AllRoles, Has.Count.EqualTo(2));
            CollectionAssert.AreEquivalent(new[] { 1 }, vm.SelectedRoleIds);

            var selMap = vm.AllRoles.ToDictionary(x => int.Parse(x.Value), x => x.Selected);
            Assert.IsTrue(selMap[1], "Role 1 should be selected");
            Assert.IsFalse(selMap[2], "Role 2 should not be selected");
        }

        [Test]
        public void UpdateAsync_ThrowsWhenNotFound()
        {
            var model = new VolunteerViewModel { Id = 321, SelectedRoleIds = Array.Empty<int>() };
            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _svc.UpdateAsync(model)
            );
        }

        [Test]
        public async Task UpdateAsync_UpdatesPropertiesAndRoles()
        {
            var r1 = new Role { Id = 1, Name = "A" };
            var r2 = new Role { Id = 2, Name = "B" };
            var r3 = new Role { Id = 3, Name = "C" };
            _db.Roles.AddRange(r1, r2, r3);

            var vol = new Volunteer
            {
                Id = 30,
                Names = "Old",
                Email = "o@x",
                Phone = "000",
                VolunteerRoles = new List<VolunteerRole>
                {
                    new VolunteerRole { RoleId = 1 },
                    new VolunteerRole { RoleId = 2 }
                }
            };
            _db.Volunteers.Add(vol);
            await _db.SaveChangesAsync();
            _db.ChangeTracker.Clear();

            var model = new VolunteerViewModel
            {
                Id = 30,
                Names = "New",
                Email = "n@x",
                Phone = "111",
                SelectedRoleIds = new[] { 2, 3 }
            };

            await _svc.UpdateAsync(model);

            var updated = await _db.Volunteers
                .Include(v => v.VolunteerRoles)
                .FirstAsync(v => v.Id == 30);

            Assert.AreEqual("New", updated.Names);
            Assert.AreEqual("n@x", updated.Email);
            Assert.AreEqual("111", updated.Phone);

            CollectionAssert.AreEquivalent(
                new[] { 2, 3 },
                updated.VolunteerRoles.Select(vr => vr.RoleId)
            );
        }

        [Test]
        public void DeleteAsync_ThrowsWhenNotFound()
        {
            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _svc.DeleteAsync(1234)
            );
        }


    }
}
