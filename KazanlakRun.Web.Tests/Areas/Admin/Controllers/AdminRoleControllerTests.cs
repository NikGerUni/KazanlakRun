using KazanlakRun.Data.Models;
using KazanlakRun.Web.Areas.Admin.Controllers;
using KazanlakRun.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KazanlakRun.Web.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class AdminRoleControllerTests
    {
        private DbContextOptions<ApplicationDbContext> _dbOptions = null!;
        private ApplicationDbContext _context = null!;
        private RoleController _controller = null!;

        [SetUp]
        public void SetUp()
        {
            // Всеки път нов in-memory database с уникално име (за изолация)
            _dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(_dbOptions);
            _controller = new RoleController(_context);
        }

        [TearDown]
        public void TearDown()
        {
            // Освобождаваме context-а
            _context.Dispose();
            _controller.Dispose();
        }

        [Test]
        public async Task Index_WhenCalled_ReturnsViewWithAllRoles()
        {
            // Arrange: seed-ване на две роли
            var rolesSeed = new List<Role>
            {
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "User" }
            };
            _context.Roles.AddRange(rolesSeed);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result, "Index трябва да върне ViewResult");

            var model = result.Model as List<RoleViewModel>;
            Assert.IsNotNull(model, "Model трябва да е List<RoleViewModel>");
            Assert.AreEqual(2, model.Count, "Върнатите роли трябва да са 2");

            // Проверяваме, че имената съвпадат
            CollectionAssert.AreEquivalent(
                rolesSeed.Select(r => r.Name).ToList(),
                model.Select(vm => vm.Name).ToList(),
                "Имената на ролите не съвпадат"
            );

            // Проверяваме, че и ID-тата съвпадат
            CollectionAssert.AreEquivalent(
                rolesSeed.Select(r => r.Id).ToList(),
                model.Select(vm => vm.Id).ToList(),
                "ID-тата на ролите не съвпадат"
            );
        }
        [Test]
        public async Task SaveAll_Post_NewAndExistingRoles_AddsAndUpdatesAndRedirects()
        {
            // Arrange
            var existing = new Role { Id = 1, Name = "OldName" };
            _context.Roles.Add(existing);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            var toSave = new List<RoleViewModel>
        {
            new RoleViewModel { Id = 1, Name = "NewName", IsDeleted = false },
            new RoleViewModel { Id = 0, Name = "Guest",   IsDeleted = false }
        };

            // Act
            var actionResult = await _controller.SaveAll(toSave) as RedirectToActionResult;

            // Assert redirect
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(nameof(_controller.Index), actionResult!.ActionName);

            // Assert база данни
            var allRoles = _context.Roles.AsNoTracking().ToList();
            Assert.AreEqual(2, allRoles.Count);
            var updated = allRoles.Single(r => r.Id == 1);
            Assert.AreEqual("NewName", updated.Name);
            var added = allRoles.Single(r => r.Name == "Guest");
            Assert.AreNotEqual(0, added.Id);
        }
    }
}
