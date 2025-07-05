using KazanlakRun.Areas.User.Models;
using KazanlakRun.Web.Areas.User.Services;
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

       


    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var model = await _volunteerService.GetByUserIdAsync(userId);
        if (model == null) return RedirectToAction("Create");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(VolunteerInputModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _volunteerService.UpdateAsync(userId, model);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _volunteerService.DeleteAsync(userId);
        return RedirectToAction("Index", "Home");
    }

}
}

