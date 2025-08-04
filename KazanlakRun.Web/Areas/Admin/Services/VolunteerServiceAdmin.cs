using KazanlakRun.Data.Models;
using KazanlakRun.Web.Areas.Admin.Models;
using KazanlakRun.Web.Areas.Admin.Services.IServices;
using KazanlakRun.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KazanlakRun.Web.Areas.Admin.Services
{
    public class VolunteerServiceAdmin : IVolunteerServiceAdmin
    {
        private readonly ApplicationDbContext _db;
        private readonly ICacheService _cacheService;

        public VolunteerServiceAdmin(ApplicationDbContext db, ICacheService cacheService)
            => (_db, _cacheService) = (db, cacheService);

        public async Task<List<Role>> GetAllRolesAsync()
            => await _db.Roles
                        .Select(r => new Role { Id = r.Id, Name = r.Name })
                        .ToListAsync();

        public async Task<List<VolunteerListItem>> GetAllVolunteersAsync()
            => await _db.Volunteers
                        .Include(v => v.VolunteerRoles)
                            .ThenInclude(vr => vr.Role)
                        .Select(v => new VolunteerListItem
                        {
                            Id = v.Id,
                            Names = v.Names,
                            Email = v.Email,
                            Phone = v.Phone,
                            RoleNames = v.VolunteerRoles
                                         .Select(vr => vr.Role.Name)
                                         .ToList()
                        })
                        .ToListAsync();

        public async Task<VolunteerViewModel> GetForCreateAsync()
        {
            var roles = await GetAllRolesAsync();
            return new VolunteerViewModel
            {
                AllRoles = roles
                    .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name })
                    .ToList()
            };
        }

        public async Task CreateAsync(VolunteerViewModel model)
        {
            var entity = new Volunteer
            {
                Names = model.Names,
                Email = model.Email,
                Phone = model.Phone,
                AidStationId = 1
            };

            foreach (var rid in model.SelectedRoleIds ?? Array.Empty<int>())
                entity.VolunteerRoles.Add(new VolunteerRole { RoleId = rid });

            _db.Volunteers.Add(entity);
            await _db.SaveChangesAsync();

            _cacheService.ClearReportCache();
        }

        public async Task<VolunteerViewModel> GetForEditAsync(int id)
        {
            var vol = await _db.Volunteers
                               .Include(v => v.VolunteerRoles)
                               .FirstOrDefaultAsync(v => v.Id == id)
                       ?? throw new KeyNotFoundException();

            var roles = await GetAllRolesAsync();

            return new VolunteerViewModel
            {
                Id = vol.Id,
                Names = vol.Names,
                Email = vol.Email,
                Phone = vol.Phone,
                AllRoles = roles
                    .Select(r => new SelectListItem
                    {
                        Value = r.Id.ToString(),
                        Text = r.Name,
                        Selected = vol.VolunteerRoles.Any(vr => vr.RoleId == r.Id)
                    })
                    .ToList(),
                SelectedRoleIds = vol.VolunteerRoles
                                     .Select(vr => vr.RoleId)
                                     .ToArray()
            };
        }

        public async Task UpdateAsync(VolunteerViewModel model)
        {
            var vol = await _db.Volunteers
                               .Include(v => v.VolunteerRoles)
                               .FirstOrDefaultAsync(v => v.Id == model.Id)
                       ?? throw new KeyNotFoundException();

            vol.Names = model.Names;
            vol.Email = model.Email;
            vol.Phone = model.Phone;

            var toRemove = vol.VolunteerRoles
                              .Where(vr => !((model.SelectedRoleIds ?? Array.Empty<int>()).Contains(vr.RoleId)))
                              .ToList();

            foreach (var vr in toRemove)
                _db.Remove(vr);

            var existing = vol.VolunteerRoles.Select(vr => vr.RoleId).ToList();
            var toAdd = (model.SelectedRoleIds ?? Array.Empty<int>()).Except(existing);

            foreach (var rid in toAdd)
                vol.VolunteerRoles.Add(new VolunteerRole { RoleId = rid });

            await _db.SaveChangesAsync();

            _cacheService.ClearReportCache();
        }

        public async Task DeleteAsync(int id)
        {
            var vol = await _db.Volunteers.FindAsync(id)
                      ?? throw new KeyNotFoundException();

            _db.Volunteers.Remove(vol);
            await _db.SaveChangesAsync();

            _cacheService.ClearReportCache();
        }
    }
}
