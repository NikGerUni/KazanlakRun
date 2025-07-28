using KazanlakRun.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Good>>> GetGoods()
        {
            return await _context.Goods
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Good>> GetGood(int id)
        {
            var good = await _context.Goods.FindAsync(id);
            if (good == null)
                return NotFound();
            return good;
        }

        [HttpPost]
        public async Task<ActionResult<Good>> CreateGood([FromBody] Good good)
        {
            _context.Goods.Add(good);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetGood), new { id = good.Id }, good);
        }

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

        [HttpPost("batch")]
        public async Task<ActionResult<IEnumerable<Good>>> SaveBatch([FromBody] List<Good> goods)
        {
            try
            {
                if (goods == null || !goods.Any())
                {
                    return BadRequest("No goods provided");
                }

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

                var existingGoods = await _context.Goods.ToListAsync();
                var existingIds = existingGoods.Select(g => g.Id).ToHashSet();
                var incomingIds = goods.Select(g => g.Id).ToHashSet();

                var goodsToAdd = goods.Where(g => g.Id <= 0 || !existingIds.Contains(g.Id)).ToList();

                var goodsToUpdate = goods.Where(g => g.Id > 0 && existingIds.Contains(g.Id)).ToList();

                var goodsToDelete = existingGoods.Where(g => !incomingIds.Contains(g.Id)).ToList();

                if (goodsToDelete.Any())
                {
                    _context.Goods.RemoveRange(goodsToDelete);
                }

                foreach (var good in goodsToUpdate)
                {
                    var existingGood = existingGoods.First(g => g.Id == good.Id);
                    existingGood.Name = good.Name;
                    existingGood.Measure = good.Measure;
                    existingGood.Quantity = good.Quantity;
                    existingGood.QuantityOneRunner = good.QuantityOneRunner;

                }

                foreach (var good in goodsToAdd)
                {
                    good.Id = 0;
                }
                await _context.Goods.AddRangeAsync(goodsToAdd);

                await _context.SaveChangesAsync();

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