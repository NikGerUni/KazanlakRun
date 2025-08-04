using KazanlakRun.Data.Models;
using KazanlakRun.Web.Areas.Admin.Models;
using KazanlakRun.Web.Areas.Admin.Services.IServices;
using KazanlakRun.Web.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace KazanlakRun.Web.Areas.Admin.Services
{
    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RoleService> _logger;
        private readonly ICacheService _cacheService;

        public RoleService(ApplicationDbContext context, ILogger<RoleService> logger, ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _cacheService = cacheService;
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
            IDbContextTransaction? tx = null;
            try
            {
                tx = await _context.Database.BeginTransactionAsync();

                var toDeleteIds = roles
                    .Where(r => r.IsDeleted && r.Id > 0)
                    .Select(r => r.Id)
                    .ToList();

                if (toDeleteIds.Any())
                {
                    var entitiesToDelete = await _context.Roles
                        .Where(r => toDeleteIds.Contains(r.Id))
                        .ToListAsync();

                    _context.Roles.RemoveRange(entitiesToDelete);
                    _logger.LogInformation("Removing {Count} roles", entitiesToDelete.Count);
                }

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
                            _logger.LogWarning("Role {Id} not found, skipping update", vm.Id);
                            continue;
                        }
                        entity.Name = vm.Name;
                        _context.Entry(entity)
                                .Property(e => e.Name)
                                .IsModified = true;
                        _logger.LogInformation("Updated role {Id} → '{Name}'", vm.Id, vm.Name);
                    }
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                // Invalidate relevant cache after successful save
                _cacheService.ClearReportCache();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SaveAllAsync");
                if (tx != null)
                {
                    try { await tx.RollbackAsync(); }
                    catch { /* ignore rollback errors */ }
                }
                if (ex is DbUpdateException)
                    throw;
                throw new DbUpdateException("An error occurred while saving roles.", ex);
            }
        }
    }
}
