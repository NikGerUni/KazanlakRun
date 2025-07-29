using KazanlakRun.Data;
using KazanlakRun.Data.Models;
using KazanlakRun.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KazanlakRun.Services
{
    public class GoodsService : IGoodsService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GoodsService> _logger;

        public GoodsService(ApplicationDbContext context, ILogger<GoodsService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Good>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all goods...");
            return await _context.Goods.AsNoTracking().ToListAsync();
        }

        public async Task<Good?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching good with ID {Id}", id);
            return await _context.Goods.FindAsync(id);
        }

        public async Task<Good> CreateAsync(Good good)
        {
            _context.Goods.Add(good);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Created good with ID {Id}", good.Id);
            return good;
        }

        public async Task<bool> UpdateAsync(Good good)
        {
            if (!_context.Goods.Any(g => g.Id == good.Id))
            {
                _logger.LogWarning("Update failed: Good with ID {Id} not found", good.Id);
                return false;
            }

            _context.Entry(good).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated good with ID {Id}", good.Id);
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error while updating good with ID {Id}", good.Id);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var good = await _context.Goods.FindAsync(id);
            if (good == null)
            {
                _logger.LogWarning("Delete failed: Good with ID {Id} not found", id);
                return false;
            }

            _context.Goods.Remove(good);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted good with ID {Id}", id);
            return true;
        }

        public async Task<List<Good>> SaveBatchAsync(List<Good> goods)
        {
            _logger.LogInformation("Saving batch of {Count} goods", goods.Count);

            var existingGoods = await _context.Goods.ToListAsync();
            var existingIds = existingGoods.Select(g => g.Id).ToHashSet();
            var incomingIds = goods.Select(g => g.Id).ToHashSet();

            var goodsToAdd = goods.Where(g => g.Id <= 0 || !existingIds.Contains(g.Id)).ToList();
            var goodsToUpdate = goods.Where(g => g.Id > 0 && existingIds.Contains(g.Id)).ToList();
            var goodsToDelete = existingGoods.Where(g => !incomingIds.Contains(g.Id)).ToList();

            if (goodsToDelete.Any())
            {
                _context.Goods.RemoveRange(goodsToDelete);
                _logger.LogInformation("Marked {Count} goods for deletion", goodsToDelete.Count);
            }

            foreach (var good in goodsToUpdate)
            {
                var existing = existingGoods.First(g => g.Id == good.Id);
                existing.Name = good.Name;
                existing.Measure = good.Measure;
                existing.Quantity = good.Quantity;
                existing.QuantityOneRunner = good.QuantityOneRunner;
            }

            foreach (var good in goodsToAdd)
            {
                good.Id = 0; // ensure it's treated as new
            }

            await _context.Goods.AddRangeAsync(goodsToAdd);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Batch save completed. Added: {Added}, Updated: {Updated}, Deleted: {Deleted}",
                goodsToAdd.Count, goodsToUpdate.Count, goodsToDelete.Count);

            return await _context.Goods.AsNoTracking().ToListAsync();
        }
    }
}

