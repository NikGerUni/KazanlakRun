using KazanlakRun.Web.Areas.Public.Controllers;
using KazanlakRun.Web.Areas.Public.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace KazanlakRun.Web.Tests.Controllers
{
    [TestFixture]
    public class HomeControllerTests
    {
        private HomeController _controller;
        private DefaultHttpContext _httpContext;

        [SetUp]
        public void SetUp()
        {
            _controller = new HomeController();
            _httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContext
            };
        }

        [TearDown]
        public void TearDown()
        {
            if (_controller is IDisposable disposable)
                disposable.Dispose();
        }

        private void SetUser(bool isAuthenticated, params string[] roles)
        {
            var claims = new List<Claim>();
            if (isAuthenticated)
            {
                claims.Add(new Claim(ClaimTypes.Name, "testuser"));
                foreach (var role in roles)
                    claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, isAuthenticated ? "TestAuthType" : null);
            var principal = new ClaimsPrincipal(identity);
            _httpContext.User = principal;
        }

        [Test]
        public void Index_UserNotAuthenticated_ReturnsDefaultView()
        {
            SetUser(isAuthenticated: false);

            var result = _controller.Index();

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void Index_AuthenticatedAdmin_RedirectsToAdminArea()
        {
            SetUser(isAuthenticated: true, roles: new[] { "Admin" });

            var result = _controller.Index() as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
            Assert.AreEqual("Admin", result.RouteValues["area"]);
        }

        [Test]
        public void Index_AuthenticatedUser_RedirectsToUserArea()
        {
            SetUser(isAuthenticated: true, roles: new[] { "User" });

            var result = _controller.Index() as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
            Assert.AreEqual("User", result.RouteValues["area"]);
        }

        [Test]
        public void Index_AuthenticatedNoRole_RedirectsToUserArea()
        {
            SetUser(isAuthenticated: true /*, no roles */);

            var result = _controller.Index() as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
            Assert.AreEqual("User", result.RouteValues["area"]);
        }

        [Test]
        public void Privacy_ReturnsView()
        {
            SetUser(isAuthenticated: false);

            var result = _controller.Privacy();

            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void Error_ReturnsViewWithErrorViewModel()
        {
            _httpContext.TraceIdentifier = "trace-123";


            var result = _controller.Error() as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ErrorViewModel>(result.Model);

            var model = (ErrorViewModel)result.Model;
            Assert.AreEqual("trace-123", model.RequestId);
        }
    }
}

