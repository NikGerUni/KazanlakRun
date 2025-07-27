using KazanlakRun.Web.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace KazanlakRun.Web.Tests.Areas.Admin.Controllers
{
    [TestFixture]
    public class AdminGoodsControllerTests
    {
        [Test]
        public void Index_ShouldReturnDefaultView()
        {
            // Arrange
            var controller = new GoodsController();

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result, "Index() трябва да връща ViewResult");
            var view = (ViewResult)result;
            Assert.IsTrue(string.IsNullOrEmpty(view.ViewName),
                "При липса на изрично ViewName, трябва да е празно по подразбиране");
        }

        [Test]
        public void Controller_HasAreaAttribute_Admin()
        {
            // Act
            var attr = typeof(GoodsController)
                .GetCustomAttributes(typeof(AreaAttribute), inherit: false)
                .Cast<AreaAttribute>()
                .SingleOrDefault();

            // Assert
            Assert.IsNotNull(attr, "Липсва [Area] атрибут");
            Assert.AreEqual("Admin", attr.RouteValue,
                "AreaAttribute.RouteValue трябва да бъде \"Admin\"");
        }

        [Test]
        public void Controller_HasRouteAttribute_CorrectTemplate()
        {
            // Act
            var routeAttr = typeof(GoodsController)
                .GetCustomAttributes(typeof(RouteAttribute), inherit: false)
                .Cast<RouteAttribute>()
                .SingleOrDefault();

            // Assert
            Assert.IsNotNull(routeAttr, "Липсва [Route] атрибут");
            Assert.AreEqual("Admin/[controller]/[action]", routeAttr.Template,
                "RouteAttribute.Template трябва да е \"Admin/[controller]/[action]\"");
        }
    }
}
