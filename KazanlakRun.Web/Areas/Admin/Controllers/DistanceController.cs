using KazanlakRun.Data.Models;
using KazanlakRun.Web.Areas.Admin.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace KazanlakRun.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DistanceController : Controller
    {
        private readonly IDistanceService _svc;
        public DistanceController(IDistanceService svc)
            => _svc = svc;

        public async Task<IActionResult> EditAll()
        {
            var list = (await _svc.GetAllAsync()).ToList();
            return View(list);
        }

        // POST: Admin/Distance/EditAll
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAll(List<Distance> model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _svc.UpdateMultipleAsync(model);
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }



        // GET: Admin/Distance
        public async Task<IActionResult> Index()
        {
            var list = await _svc.GetAllAsync();
            return View(list);
        }

        // GET: Admin/Distance/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _svc.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return View(dto);
        }

        // POST: Admin/Distance/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Distance model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _svc.UpdateAsync(model);
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }
    }
}

