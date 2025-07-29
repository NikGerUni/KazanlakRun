using KazanlakRun.Data.Models;
using KazanlakRun.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace KazanlakRun.Web.Areas.Admin.Controllers
{
    [ApiController]
    [Area("Admin")]
    [Route("api/[controller]")]
    public class GoodsApiController : ControllerBase
    {
        private readonly IGoodsService _goodsService;
        private readonly ILogger<GoodsApiController> _logger;

        public GoodsApiController(IGoodsService goodsService, ILogger<GoodsApiController> logger)
        {
            _goodsService = goodsService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Good>>> GetGoods()
        {
            return Ok(await _goodsService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Good>> GetGood(int id)
        {
            var good = await _goodsService.GetByIdAsync(id);
            if (good == null) return NotFound();
            return Ok(good);
        }

        [HttpPost]
        public async Task<ActionResult<Good>> CreateGood([FromBody] Good good)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _goodsService.CreateAsync(good);
            return CreatedAtAction(nameof(GetGood), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGood(int id, [FromBody] Good good)
        {
            if (id != good.Id)
                return BadRequest("ID mismatch");

            var success = await _goodsService.UpdateAsync(good);
            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGood(int id)
        {
            var success = await _goodsService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }

        [HttpPost("batch")]
        public async Task<ActionResult<IEnumerable<Good>>> SaveBatch([FromBody] List<Good> goods)
        {
            if (goods == null || !goods.Any())
                return BadRequest("No goods provided");

            var result = await _goodsService.SaveBatchAsync(goods);
            return Ok(result);
        }
    }
}
