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
            var controller = new GoodsController();

            var result = controller.Index();

            Assert.IsInstanceOf<ViewResult>(result, "Index() should return a ViewResult");
            var view = (ViewResult)result;
            Assert.IsTrue(string.IsNullOrEmpty(view.ViewName),
                "When no explicit ViewName is provided, it should be null or empty by default");
        }

        [Test]
        public void Controller_HasAreaAttribute_Admin()
        {
            var attr = typeof(GoodsController)
                .GetCustomAttributes(typeof(AreaAttribute), inherit: false)
                .Cast<AreaAttribute>()
                .SingleOrDefault();

            Assert.IsNotNull(attr, "Missing [Area] attribute");
            Assert.AreEqual("Admin", attr.RouteValue,
                "AreaAttribute.RouteValue should be \"Admin\"");
        }

        [Test]
        public void Controller_HasRouteAttribute_CorrectTemplate()
        {
            var routeAttr = typeof(GoodsController)
                .GetCustomAttributes(typeof(RouteAttribute), inherit: false)
                .Cast<RouteAttribute>()
                .SingleOrDefault();

            Assert.IsNotNull(routeAttr, "Missing [Route] attribute");
            Assert.AreEqual("Admin/[controller]/[action]", routeAttr.Template,
                "RouteAttribute.Template should be \"Admin/[controller]/[action]\"");
        }
    }
}
