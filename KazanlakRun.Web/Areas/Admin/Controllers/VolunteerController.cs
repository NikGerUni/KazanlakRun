using KazanlakRun.Web.Areas.Admin.Models;
using KazanlakRun.Web.Areas.Admin.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace KazanlakRun.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class VolunteerController : Controller
    {
        private readonly IVolunteerServiceAdmin _volunteerService;

        public VolunteerController(IVolunteerServiceAdmin volunteerService)
            => _volunteerService = volunteerService;

        public async Task<IActionResult> Index()
        {
            var list = await _volunteerService.GetAllVolunteersAsync();
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            var vm = await _volunteerService.GetForCreateAsync();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VolunteerViewModel model)
        {
            if (!ModelState.IsValid)
            {
             
                var vm = await _volunteerService.GetForCreateAsync();
                model.AllRoles = vm.AllRoles;
                return View(model);
            }

            await _volunteerService.CreateAsync(model);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _volunteerService.GetForEditAsync(id);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VolunteerViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _volunteerService.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var vm = await _volunteerService.GetForEditAsync(id);
            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _volunteerService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

