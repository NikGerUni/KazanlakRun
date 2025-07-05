using KazanlakRun.Data.Models;
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
        {
            _context = context;
        }

        // GET: Admin/Role
        public async Task<IActionResult> Index()
        {
            var roles = await _context.Roles.AsNoTracking().ToListAsync();
            return View(roles);
        }

        // POST: Admin/Role/SaveAll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAll(List<Role> roles)
        {
            foreach (var r in roles)
            {
                if (r.Id == 0)
                    _context.Roles.Add(r);
                else
                    _context.Roles.Update(r);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
