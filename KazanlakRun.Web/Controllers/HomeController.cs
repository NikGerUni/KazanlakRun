namespace KazanlakRun.Web.Controllers
{
    using KazanlakRun.Web.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
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
                    return RedirectToAction("Index", "Home", new { area = "User" });
                }
            }

            return View();
        }
        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Map()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}



