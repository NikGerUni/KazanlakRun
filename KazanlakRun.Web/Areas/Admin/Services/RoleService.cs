using KazanlakRun.Data;
using KazanlakRun.Data.Models;
using KazanlakRun.Web.Areas.Admin.Models;
using KazanlakRun.Web.Areas.Admin.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KazanlakRun.Web.Areas.Admin.Services
{
    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RoleService> _logger;

        public RoleService(ApplicationDbContext context, ILogger<RoleService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<RoleViewModel>> GetAllAsync()
        {
            return await _context.Roles
                                 .AsNoTracking()
                                 .Select(r => new RoleViewModel
                                 {
                                     Id = r.Id,
                                     Name = r.Name,
                                     IsDeleted = false
                                 })
                                 .ToListAsync();
        }

        public async Task SaveAllAsync(List<RoleViewModel> roles)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1) Изтриване
                var toDelete = roles.Where(r => r.IsDeleted && r.Id > 0)
                                    .Select(r => new Role { Id = r.Id });
                _context.Roles.RemoveRange(toDelete);
                _logger.LogInformation("Removing {Count} roles", toDelete.Count());

                // 2) Добавяне или Update върху проследени (tracked) обекти
                foreach (var vm in roles.Where(r => !r.IsDeleted))
                {
                    if (vm.Id == 0)
                    {
                        _context.Roles.Add(new Role { Name = vm.Name });
                        _logger.LogInformation("Adding new role '{Name}'", vm.Name);
                    }
                    else
                    {
                        var entity = await _context.Roles.FindAsync(vm.Id);
                        if (entity == null)
                        {
                            _logger.LogWarning("Role {Id} not found, skipping", vm.Id);
                            continue;
                        }
                        entity.Name = vm.Name;
                        _context.Entry(entity).Property(e => e.Name).IsModified = true;
                        _logger.LogInformation("Updated role {Id} → '{Name}'", vm.Id, vm.Name);
                    }
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SaveAllAsync");
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}

