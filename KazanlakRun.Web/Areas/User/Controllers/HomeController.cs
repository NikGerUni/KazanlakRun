using KazanlakRun.Areas.User.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
            bool volunteerExists = false;
            
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                volunteerExists = await _volunteerService.ExistsAsync(userId);
            

            return View(volunteerExists);
        }
    }
}

