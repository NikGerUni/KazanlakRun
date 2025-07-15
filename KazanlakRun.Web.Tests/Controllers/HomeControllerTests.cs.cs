using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using KazanlakRun.Web.Controllers;
using KazanlakRun.Web.ViewModels;


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
            // Arrange
            SetUser(isAuthenticated: false);

            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void Index_AuthenticatedAdmin_RedirectsToAdminArea()
        {
            // Arrange
            SetUser(isAuthenticated: true, roles: new[] { "Admin" });

            // Act
            var result = _controller.Index() as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
            Assert.AreEqual("Admin", result.RouteValues["area"]);
        }

        [Test]
        public void Index_AuthenticatedUser_RedirectsToUserArea()
        {
            // Arrange
            SetUser(isAuthenticated: true, roles: new[] { "User" });

            // Act
            var result = _controller.Index() as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
            Assert.AreEqual("User", result.RouteValues["area"]);
        }

        [Test]
        public void Index_AuthenticatedNoRole_RedirectsToUserArea()
        {
            // Arrange
            SetUser(isAuthenticated: true /*, no roles */);

            // Act
            var result = _controller.Index() as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
            Assert.AreEqual("User", result.RouteValues["area"]);
        }

        [Test]
        public void Privacy_ReturnsView()
        {
            // Arrange
            SetUser(isAuthenticated: false);

            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void Error_ReturnsViewWithErrorViewModel()
        {
            // Arrange
            // Задаваме TraceIdentifier, така че да очакваме именно него
            _httpContext.TraceIdentifier = "trace-123";

            // Не задаваме Activity.Current → RequestId ще дойде от HttpContext.TraceIdentifier

            // Act
            var result = _controller.Error() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ErrorViewModel>(result.Model);

            var model = (ErrorViewModel)result.Model;
            Assert.AreEqual("trace-123", model.RequestId);
        }
    }
}

