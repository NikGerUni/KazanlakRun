using KazanlakRun.Web.Areas.Admin.Models;
using KazanlakRun.Web.Areas.Admin.Services.IServices;
using KazanlakRun.Web.Areas.User.Models;
using KazanlakRun.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace KazanlakRun.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [ServiceFilter(typeof(VolunteerExceptionFilter))]
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

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(VolunteerViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //        return View(model);

        //    await _volunteerService.UpdateAsync(model);
        //    return RedirectToAction(nameof(Index));
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VolunteerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            try
            {
                await _volunteerService.UpdateAsync( model);
                return RedirectToAction("Index", "Home");
            }
            catch (DbUpdateException ex) when (
                ex.InnerException?.Message.Contains("IX_Volunteers_Email") == true)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "Email must be unique");
                     return View(model);
            }
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

