using KazanlakRun.Areas.User.Models;
using KazanlakRun.Areas.User.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KazanlakRun.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User")]
    public class VolunteerController : Controller
    {
        private readonly IVolunteerService _volunteerService;

        public VolunteerController(IVolunteerService volunteerService)
        {
            _volunteerService = volunteerService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(VolunteerInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            await _volunteerService.CreateAsync(userId, model);

            return RedirectToAction("Index", "Home");
        }
    }
}

