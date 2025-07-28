using KazanlakRun.Web.Areas.Admin.Models;
using KazanlakRun.Web.Areas.Admin.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KazanlakRun.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DistanceController : Controller
    {
        private readonly IDistanceEditDtoService _svc;
        public DistanceController(IDistanceEditDtoService svc)
            => _svc = svc;

        public async Task<IActionResult> EditAll()
        {
            var list = (await _svc.GetAllAsync()).ToList();
            return View(list);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAll(List<DistanceEditDto> model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _svc.UpdateMultipleAsync(model);
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }



        public async Task<IActionResult> Index()
        {
            var list = await _svc.GetAllAsync();
            return View(list);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _svc.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return View(dto);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DistanceEditDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _svc.UpdateAsync(model);
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }
    }
}

