using KazanlakRun.Data.Models;
using Microsoft.EntityFrameworkCore;
using KazanlakRun.Web.Areas.Admin.Services.IServices;

namespace KazanlakRun.Web.Areas.Admin.Services
{
    public class DistanceService : IServices.IDistanceService
    {
        private readonly ApplicationDbContext _db;
        public DistanceService(ApplicationDbContext db) => _db = db;

        public async Task<IEnumerable<Distance>> GetAllAsync()
            => await _db.Distances.AsNoTracking().ToListAsync();

        public async Task<Distance?> GetByIdAsync(int id)
            => await _db.Distances.FindAsync(id);

        public async Task UpdateAsync(Distance distance)
        {
            _db.Distances.Update(distance);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateMultipleAsync(IEnumerable<Distance> distances)
        {
            // Метод 1: Обновяване на отделни записи
            foreach (var distance in distances)
            {
                var existingEntity = await _db.Distances.FindAsync(distance.Id);
                if (existingEntity != null)
                {
                    existingEntity.RegRunners = distance.RegRunners;
                    // Обновете и други полета при нужда
                    // existingEntity.Distans = distance.Distans;
                }
            }
            await _db.SaveChangesAsync();

            // Алтернативен метод 2: Ако искате да използвате UpdateRange
            /*
            _db.Distances.UpdateRange(distances);
            await _db.SaveChangesAsync();
            */
        }
    }
}