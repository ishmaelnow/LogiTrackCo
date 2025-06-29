using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogiTrack.Models;

namespace LogiTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly LogiTrackContext _context;

        public InventoryController(LogiTrackContext context)
        {
            _context = context;
        }

        // ✅ Public GET - anyone can see inventory
        // ✅ Require JWT for GET all inventory for now to test authorization
        // ✅ Later, we can remove the JWT requirement for public access
        [Authorize]
        [HttpGet]
        public ActionResult<IEnumerable<InventoryItem>> GetInventory()
        {
            var items = _context.InventoryItems.Include(i => i.Order).ToList();
            return Ok(items);
        }

        // ✅ Public GET by ID
        [HttpGet("{id}")]
        public ActionResult<InventoryItem> GetInventoryItem(int id)
        {
            var item = _context.InventoryItems.Include(i => i.Order).FirstOrDefault(i => i.ItemId == id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        // ✅ Create - Only Admins can add inventory
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<InventoryItem> CreateInventoryItem([FromBody] InventoryItem newItem)
        {
            _context.InventoryItems.Add(newItem);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetInventoryItem), new { id = newItem.ItemId }, newItem);
        }

        // ✅ Update - Only Admins
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateInventoryItem(int id, [FromBody] InventoryItem updatedItem)
        {
            var existingItem = _context.InventoryItems.Find(id);
            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.Name = updatedItem.Name;
            existingItem.Quantity = updatedItem.Quantity;
            existingItem.Location = updatedItem.Location;
            existingItem.OrderId = updatedItem.OrderId;

            _context.SaveChanges();
            return NoContent();
        }

        // ✅ Delete - Only Admins
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteInventoryItem(int id)
        {
            var item = _context.InventoryItems.Find(id);
            if (item == null)
            {
                return NotFound();
            }

            _context.InventoryItems.Remove(item);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
