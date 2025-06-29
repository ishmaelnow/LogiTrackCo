using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using LogiTrack.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace LogiTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // ✅ Require valid JWT for all inventory endpoints
    public class InventoryController : ControllerBase
    {
        private readonly LogiTrackContext _context;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(LogiTrackContext context, ILogger<InventoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ✅ Get all inventory items (Authorized users)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItem>>> GetInventory()
        {
            _logger.LogInformation("GET /api/inventory called ✅");

            var items = await _context.InventoryItems
                .Include(i => i.Order)
                .ToListAsync();

            return Ok(items);
        }

        // ✅ Get specific item by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryItem>> GetInventoryItem(int id)
        {
            _logger.LogInformation($"GET /api/inventory/{id} called ✅");

            var item = await _context.InventoryItems
                .Include(i => i.Order)
                .FirstOrDefaultAsync(i => i.ItemId == id);

            if (item == null)
            {
                _logger.LogWarning($"Item with ID {id} not found ❌");
                return NotFound();
            }

            return Ok(item);
        }

        // ✅ Create a new inventory item (Admin only)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<InventoryItem>> CreateInventoryItem([FromBody] InventoryItem newItem)
        {
            _logger.LogInformation("POST /api/inventory called ✅");

            _context.InventoryItems.Add(newItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetInventoryItem), new { id = newItem.ItemId }, newItem);
        }

        // ✅ Delete inventory item by ID (Admin only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteInventoryItem(int id)
        {
            _logger.LogInformation($"DELETE /api/inventory/{id} called ✅");

            var item = await _context.InventoryItems.FindAsync(id);
            if (item == null)
            {
                _logger.LogWarning($"Item with ID {id} not found ❌");
                return NotFound();
            }

            _context.InventoryItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
