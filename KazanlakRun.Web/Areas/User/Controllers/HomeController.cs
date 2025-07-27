using KazanlakRun.Web.Areas.User.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KazanlakRun.Web.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User")]
    public class HomeController : Controller
    {
        private readonly IVolunteerService _volunteerService;

        public HomeController(IVolunteerService volunteerService)
        {
            _volunteerService = volunteerService;
        }


        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            var volunteerExists = await _volunteerService.ExistsAsync(userId);
            return View(volunteerExists);
        }


    }
}

