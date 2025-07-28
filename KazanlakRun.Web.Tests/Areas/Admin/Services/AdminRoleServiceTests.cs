using KazanlakRun.Data.Models;
using KazanlakRun.Web.Areas.Admin.Models;
using KazanlakRun.Web.Areas.Admin.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;

namespace KazanlakRun.Web.Tests.Areas.Admin.Services
{
    [TestFixture]
    public class AdminRoleServiceTests
    {
        private ApplicationDbContext _context;
        private RoleService _service;
        private Mock<ILogger<RoleService>> _logger;

        [SetUp]

        public void SetUp()
        {
            var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new ApplicationDbContext(opts);
            _logger = new Mock<ILogger<RoleService>>();
            _service = new RoleService(_context, _logger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetAllAsync_ReturnsAllRoles_AsViewModels()
        {
            _context.Roles.AddRange(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "User" }
            );
            await _context.SaveChangesAsync();

            var all = await _service.GetAllAsync();

            Assert.AreEqual(2, all.Count);
            CollectionAssert.AreEquivalent(
                new[] { "Admin", "User" },
                all.Select(r => r.Name));
        }

        [Test]
        public async Task SaveAllAsync_AddsNewRole_WhenIdIsZero()
        {
            var vm = new List<RoleViewModel>
            {
                new() { Id = 0, Name = "NewRole", IsDeleted = false }
            };

            await _service.SaveAllAsync(vm);

            var inDb = await _context.Roles.SingleAsync();
            Assert.AreEqual("NewRole", inDb.Name);
        }

        [Test]
        public async Task SaveAllAsync_UpdatesExistingRole_WhenIdMatches()
        {
            var existing = new Role { Name = "OldName" };
            _context.Roles.Add(existing);
            await _context.SaveChangesAsync();

            var vm = new List<RoleViewModel>
            {
                new() { Id = existing.Id, Name = "NewName", IsDeleted = false }
            };

            await _service.SaveAllAsync(vm);

            var inDb = await _context.Roles.FindAsync(existing.Id);
            Assert.AreEqual("NewName", inDb.Name);
        }

        [Test]
        public async Task SaveAllAsync_DeletesMarkedRoles()
        {
            var keep = new Role { Name = "Keep" };
            var del = new Role { Name = "Del" };
            _context.Roles.AddRange(keep, del);
            await _context.SaveChangesAsync();

            var vm = new List<RoleViewModel>
            {
                new() { Id = keep.Id, Name = keep.Name, IsDeleted = false },
                new() { Id = del.Id,  Name = del.Name,  IsDeleted = true  }
            };

            await _service.SaveAllAsync(vm);

            var names = (await _context.Roles.ToListAsync()).Select(r => r.Name);
            CollectionAssert.AreEquivalent(new[] { "Keep" }, names);
        }


    }
}