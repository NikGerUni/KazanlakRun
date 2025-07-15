using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KazanlakRun.Data.Models;
using KazanlakRun.Web.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

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
            var model = result!.Model as List<Role>;
            Assert.IsNotNull(model, "Model трябва да е List<Role>");
            Assert.AreEqual(2, model!.Count, "Върнатите роли трябва да са 2");
            // Проверяваме, че съдържа точно нашите
            CollectionAssert.AreEquivalent(
                rolesSeed.Select(r => r.Name),
                model.Select(r => r.Name)
            );
        }

        [Test]
        public async Task SaveAll_Post_NewAndExistingRoles_AddsAndUpdatesAndRedirects()
        {
            // Arrange:
            // 1) добавяме една вече съществуваща роля
            var existing = new Role { Id = 1, Name = "OldName" };
            _context.Roles.Add(existing);
            await _context.SaveChangesAsync();
            // След SaveChangesAsync():
            _context.ChangeTracker.Clear();


            // 2) подготвяме списък за SaveAll:
            //    - обновяваме съществуващата
            //    - добавяме нова (Id = 0)
            var toSave = new List<Role>
            {
                new Role { Id = 1, Name = "NewName" },
                new Role { Id = 0, Name = "Guest" }
            };

            // Act
            var actionResult = await _controller.SaveAll(toSave) as RedirectToActionResult;

            // Assert (редирект)
            Assert.IsNotNull(actionResult, "SaveAll трябва да редиректне");
            Assert.AreEqual(nameof(_controller.Index),
                            actionResult!.ActionName,
                            "ActionName трябва да е Index");

            // Assert (база)
            var allRoles = _context.Roles.AsNoTracking().ToList();
            Assert.AreEqual(2, allRoles.Count, "В базата трябва да има 2 роли");

            // – проверяваме, че съществуващата е обновена
            var updated = allRoles.Single(r => r.Id == 1);
            Assert.AreEqual("NewName", updated.Name, "Съществуващата роля трябва да е с новото име");

            // – проверяваме, че новата е добавена
            var added = allRoles.Single(r => r.Name == "Guest");
            Assert.AreNotEqual(0, added.Id, "Добавената роля трябва да има генерирано Id");
        }
    }
}
