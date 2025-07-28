using KazanlakRun.Web.Areas.Admin.Models;
using KazanlakRun.Web.Areas.Admin.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KazanlakRun.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RoleController> _logger;

        public RoleController(IRoleService roleService, ILogger<RoleController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleService.GetAllAsync();
            return View(roles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAll(List<RoleViewModel> roles)
        {
            if (!ModelState.IsValid)
                return View(nameof(Index), roles);

            try
            {
                await _roleService.SaveAllAsync(roles);
                TempData["Success"] = "Changes saved.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save roles");
                ModelState.AddModelError(string.Empty, "An error occurred. Please try again later.");
                return View(nameof(Index), roles);
            }
        }

        [HttpGet]
        public IActionResult RowTemplate()
        {
            ViewData["idx"] = "__index__";
            return PartialView("_RoleRow", new RoleViewModel());
        }
    }
}
