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
            // Remove all existing records
            _context.Goods.RemoveRange(_context.Goods);
            await _context.SaveChangesAsync();

            // Add the new batch
            await _context.Goods.AddRangeAsync(goods);
            await _context.SaveChangesAsync();

            return Ok(goods);
        }
    }
}

