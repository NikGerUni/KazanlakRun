using KazanlakRun.Web.Areas.Admin.Models;
using KazanlakRun.Web.Areas.Admin.Services.IServices;
using KazanlakRun.Web.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace KazanlakRun.Web.Areas.Admin.Services
{
    public class DistanceEditDtoService : IDistanceEditDtoService
    {
        private readonly ApplicationDbContext _db;
        private readonly ICacheService _cacheService;

        public DistanceEditDtoService(ApplicationDbContext db, ICacheService cacheService)
        {
            _db = db;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<DistanceEditDto>> GetAllAsync()
            => await _db.Distances
                        .AsNoTracking()
                        .Select(d => new DistanceEditDto
                        {
                            Id = d.Id,
                            Distans = d.Distans,
                            RegRunners = d.RegRunners
                        })
                        .ToListAsync();

        public async Task<DistanceEditDto?> GetByIdAsync(int id)
            => await _db.Distances
                        .AsNoTracking()
                        .Where(d => d.Id == id)
                        .Select(d => new DistanceEditDto
                        {
                            Id = d.Id,
                            Distans = d.Distans,
                            RegRunners = d.RegRunners
                        })
                        .FirstOrDefaultAsync();

        public async Task UpdateAsync(DistanceEditDto dto)
        {
            var entity = await _db.Distances.FindAsync(dto.Id);
            if (entity is null)
                throw new KeyNotFoundException($"Distance with Id={dto.Id} not found.");

            entity.RegRunners = dto.RegRunners;
            await _db.SaveChangesAsync();

            _cacheService.ClearReportCache();
        }

        public async Task UpdateMultipleAsync(IEnumerable<DistanceEditDto> distances)
        {
            var ids = distances.Select(d => d.Id).ToList();

            var entities = await _db.Distances
                                    .Where(d => ids.Contains(d.Id))
                                    .ToListAsync();

            foreach (var dto in distances)
            {
                var entity = entities.FirstOrDefault(e => e.Id == dto.Id);
                if (entity is null)
                    throw new InvalidOperationException($"Expected Distance with Id={dto.Id}, but not found.");

                entity.RegRunners = dto.RegRunners;
            }

            await _db.SaveChangesAsync();

            _cacheService.ClearReportCache();
        }

    }
}
