using KazanlakRun.Web.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KazanlakRun.Web.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class AdminHomeControllerTests
    {
        [Test]
        public void Index_ReturnsDefaultView()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result, "Index() трябва да връща ViewResult");
            var view = (ViewResult)result;
            Assert.IsTrue(string.IsNullOrEmpty(view.ViewName),
                "Прилипналият view по подразбиране трябва да има празно ViewName");
        }

        [Test]
        public void Controller_HasAreaAttribute_Admin()
        {
            // Проверяваме, че класът е декориран с [Area("Admin")]
            var areaAttr = typeof(HomeController)
                .GetCustomAttributes(typeof(AreaAttribute), inherit: false)
                .Cast<AreaAttribute>()
                .SingleOrDefault();

            Assert.IsNotNull(areaAttr, "В контролера липсва [Area]");
            Assert.AreEqual("Admin", areaAttr.RouteValue,
                "AreaAttribute.RouteValue трябва да е \"Admin\"");
        }

        [Test]
        public void Controller_HasAuthorizeAttribute_RolesAdmin()
        {
            // Проверяваме, че класът е декориран с [Authorize(Roles = "Admin")]
            var authAttr = typeof(HomeController)
                .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: false)
                .Cast<AuthorizeAttribute>()
                .SingleOrDefault();

            Assert.IsNotNull(authAttr, "В контролера липсва [Authorize]");
            Assert.AreEqual("Admin", authAttr.Roles,
                "AuthorizeAttribute.Roles трябва да е \"Admin\"");
        }
    }
}
