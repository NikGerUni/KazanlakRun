using KazanlakRun.Web.Areas.Admin.Models;
using KazanlakRun.Web.Areas.Admin.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KazanlakRun.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AidStationController : Controller
    {
        private readonly IAidStationService _service;

        public AidStationController(IAidStationService service)
            => _service = service;

        public async Task<IActionResult> Index()
        {
            var list = await _service.GetAllAsync();
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            var vm = await _service.GetForCreateAsync();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AidStationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _service.CreateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _service.GetForEditAsync(id);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AidStationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _service.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var vm = await _service.GetForEditAsync(id);
            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
                return BadRequest();

            await _service.DeleteAsync(id.Value);
            return RedirectToAction(nameof(Index));
        }
    }
}

