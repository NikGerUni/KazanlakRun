using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KazanlakRun.Data;
using KazanlakRun.Data.Models;

namespace KazanlakRun.Web.Areas.Admin.Controllers
{
    [ApiController]
    [Area("Admin")]
    [Route("api/[controller]")]
    public class GoodsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GoodsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/GoodsApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Good>>> GetGoods()
        {
            return await _context.Goods
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        // GET: api/GoodsApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Good>> GetGood(int id)
        {
            var good = await _context.Goods.FindAsync(id);
            if (good == null)
                return NotFound();
            return good;
        }

        // POST: api/GoodsApi
        [HttpPost]
        public async Task<ActionResult<Good>> CreateGood([FromBody] Good good)
        {
            _context.Goods.Add(good);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetGood), new { id = good.Id }, good);
        }

        // PUT: api/GoodsApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGood(int id, [FromBody] Good good)
        {
            if (id != good.Id)
                return BadRequest();

            _context.Entry(good).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Goods.Any(e => e.Id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/GoodsApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGood(int id)
        {
            var good = await _context.Goods.FindAsync(id);
            if (good == null)
                return NotFound();

            _context.Goods.Remove(good);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/GoodsApi/batch
        [HttpPost("batch")]
        public async Task<ActionResult<IEnumerable<Good>>> SaveBatch([FromBody] List<Good> goods)
        {
            try
            {
                // Валидация на входните данни
                if (goods == null || !goods.Any())
                {
                    return BadRequest("No goods provided");
                }

                // Валидация на всеки запис
                foreach (var good in goods)
                {
                    if (string.IsNullOrWhiteSpace(good.Name))
                    {
                        return BadRequest($"Good name is required for ID: {good.Id}");
                    }
                    if (string.IsNullOrWhiteSpace(good.Measure))
                    {
                        return BadRequest($"Good measure is required for ID: {good.Id}");
                    }
                }

                // Вземи съществуващите записи
                var existingGoods = await _context.Goods.ToListAsync();
                var existingIds = existingGoods.Select(g => g.Id).ToHashSet();
                var incomingIds = goods.Select(g => g.Id).ToHashSet();

                // Записи за добавяне (ID = 0 или не съществуват)
                var goodsToAdd = goods.Where(g => g.Id <= 0 || !existingIds.Contains(g.Id)).ToList();

                // Записи за обновяване (съществуват и в двете колекции)
                var goodsToUpdate = goods.Where(g => g.Id > 0 && existingIds.Contains(g.Id)).ToList();

                // Записи за изтриване (съществуват в БД, но не в входящите данни)
                var goodsToDelete = existingGoods.Where(g => !incomingIds.Contains(g.Id)).ToList();

                // Изтрий записите, които не са в новите данни
                if (goodsToDelete.Any())
                {
                    _context.Goods.RemoveRange(goodsToDelete);
                }

                // Обнови съществуващите записи
                foreach (var good in goodsToUpdate)
                {
                    var existingGood = existingGoods.First(g => g.Id == good.Id);
                    existingGood.Name = good.Name;
                    existingGood.Measure = good.Measure;
                    existingGood.Quantity = good.Quantity;
                    existingGood.QuantityOneRunner = good.QuantityOneRunner;
                   
                }

                // Добави новите записи (нулирай ID за автоматично генериране)
                foreach (var good in goodsToAdd)
                {
                    good.Id = 0; // За автоматично генериране на ID
                }
                await _context.Goods.AddRangeAsync(goodsToAdd);

                // Запази всички промени
                await _context.SaveChangesAsync();

                // Върни актуализираните данни
                var result = await _context.Goods.AsNoTracking().ToListAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}