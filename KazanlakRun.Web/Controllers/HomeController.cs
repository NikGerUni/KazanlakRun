namespace KazanlakRun.Web.Controllers
{
    using System.Diagnostics;
    using ViewModels;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // ��� ������������ � ������, ����������� ��� User area
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else if (User.IsInRole("User"))
                {
                    return RedirectToAction("Index", "Home", new { area = "User" });
                }
                else
                {
                    // ��� ���� ����, �� � ������, ����������� ��� User area
                    return RedirectToAction("Index", "Home", new { area = "User" });
                }
            }

            // ��� �� � ������, �������� default Index
            return View();
        }
        [HttpGet]
        public IActionResult Privacy()
        {
            // ����� Views/Home/Privacy.cshtml
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}



