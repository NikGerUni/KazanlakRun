using KazanlakRun.Data.Models;
using KazanlakRun.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KazanlakRun.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly ApplicationDbContext _context;
        public RoleController(ApplicationDbContext context)
            => _context = context;

        // GET: Admin/Role
        public async Task<IActionResult> Index()
        {
            var roles = await _context.Roles
                                      .AsNoTracking()
                                      .Select(r => new RoleViewModel
                                      {
                                          Id = r.Id,
                                          Name = r.Name,
                                          IsDeleted = false
                                      })
                                      .ToListAsync();
            return View(roles);
        }

        // POST: Admin/Role/SaveAll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAll(List<RoleViewModel> roles)
        {
            // 1) Изтриваме маркираните за изтриване
            var toDelete = roles.Where(r => r.IsDeleted && r.Id > 0)
                                .Select(r => new Role { Id = r.Id });
            _context.Roles.RemoveRange(toDelete);

            // 2) Обновяваме/добавяме останалите
            foreach (var vm in roles.Where(r => !r.IsDeleted))
            {
                if (vm.Id == 0)
                {
                    _context.Roles.Add(new Role { Name = vm.Name });
                }
                else
                {
                    _context.Roles.Update(new Role
                    {
                        Id = vm.Id,
                        Name = vm.Name
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
