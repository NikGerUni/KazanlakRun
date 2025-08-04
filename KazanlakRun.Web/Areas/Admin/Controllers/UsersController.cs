using KazanlakRun.Web.Areas.Admin.Models;
using KazanlakRun.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KazanlakRun.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [ServiceFilter(typeof(UserExceptionFilter))]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _users;
        private readonly RoleManager<IdentityRole> _roles;

        public UsersController(
            UserManager<IdentityUser> users,
            RoleManager<IdentityRole> roles)
        {
            _users = users;
            _roles = roles;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new List<UserRoleViewModel>();

            var allUsers = _users.Users.ToList();
            var currentUserId = _users.GetUserId(User);

            foreach (var u in allUsers.Where(u => u.Id != currentUserId))
            {
                var roles = await _users.GetRolesAsync(u);
                var r = roles.FirstOrDefault() ?? "—";

                vm.Add(new UserRoleViewModel
                {
                    UserId = u.Id,
                    Email = u.Email!,
                    Role = r
                });
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {

            if (id == _users.GetUserId(User))
                return Forbid();
            var u = await _users.FindByIdAsync(id);
            if (u == null) return NotFound();


            var allRoles = _roles.Roles.Select(r => r.Name!).ToList();
            var currentRoles = await _users.GetRolesAsync(u);
            var current = currentRoles.FirstOrDefault() ?? "";

            var vm = new EditUserRoleViewModel
            {
                UserId = u.Id,
                Email = u.Email!,
                Roles = allRoles,
                SelectedRole = current
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserRoleViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Roles = _roles.Roles.Select(r => r.Name!).ToList();
                return View(vm);
            }
            if (vm.UserId == _users.GetUserId(User))
                return Forbid();

            var u = await _users.FindByIdAsync(vm.UserId);
            if (u == null) return NotFound();

            var oldRoles = await _users.GetRolesAsync(u);
            if (oldRoles.Any())
            {
                await _users.RemoveFromRolesAsync(u, oldRoles);
            }

            if (!string.IsNullOrEmpty(vm.SelectedRole))
            {
                var result = await _users.AddToRoleAsync(u, vm.SelectedRole);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    vm.Roles = _roles.Roles.Select(r => r.Name!).ToList();
                    return View(vm);
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}