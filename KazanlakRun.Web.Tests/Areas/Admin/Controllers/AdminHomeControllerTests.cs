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
            var controller = new HomeController();

            var result = controller.Index();

            Assert.IsInstanceOf<ViewResult>(result, "Index() should return a ViewResult");
            var view = (ViewResult)result;
            Assert.IsTrue(string.IsNullOrEmpty(view.ViewName),
                "The default view should have an empty ViewName when not explicitly set");
        }

        [Test]
        public void Controller_HasAreaAttribute_Admin()
        {
            var areaAttr = typeof(HomeController)
                .GetCustomAttributes(typeof(AreaAttribute), inherit: false)
                .Cast<AreaAttribute>()
                .SingleOrDefault();

            Assert.IsNotNull(areaAttr, "Missing [Area] attribute on the controller");
            Assert.AreEqual("Admin", areaAttr.RouteValue,
                "AreaAttribute.RouteValue should be \"Admin\"");
        }

        [Test]
        public void Controller_HasAuthorizeAttribute_RolesAdmin()
        {
            var authAttr = typeof(HomeController)
                .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: false)
                .Cast<AuthorizeAttribute>()
                .SingleOrDefault();

            Assert.IsNotNull(authAttr, "Missing [Authorize] attribute on the controller");
            Assert.AreEqual("Admin", authAttr.Roles,
                "AuthorizeAttribute.Roles should be \"Admin\"");
        }
    }
}
