using KazanlakRun.Web.Areas.Admin.Models;
using KazanlakRun.Web.Areas.Admin.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace KazanlakRun.Web.Areas.Admin.Services
{
    public class DistanceEditDtoService : IDistanceEditDtoService
    {
        private readonly ApplicationDbContext _db;

        public DistanceEditDtoService(ApplicationDbContext db)
            => _db = db;

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
        }

        public async Task UpdateMultipleAsync(IEnumerable<DistanceEditDto> distances)
        {
            foreach (var dto in distances)
            {
                var entity = await _db.Distances.FindAsync(dto.Id);
                if (entity is null)
                    continue;

                entity.RegRunners = dto.RegRunners;
            }
            await _db.SaveChangesAsync();
        }
    }
}
