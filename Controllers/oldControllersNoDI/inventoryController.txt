using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;  // Needed for .Include()
using LogiTrack.Models;

namespace LogiTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly LogiTrackContext _context;

        public InventoryController()
        {
            _context = new LogiTrackContext();
        }

        // ✅ Get all inventory items with their linked orders
        [HttpGet]
        public ActionResult<IEnumerable<InventoryItem>> GetInventory()
        {
            Console.WriteLine("GET /api/inventory was called ✅");

            var items = _context.InventoryItems
                .Include(i => i.Order)   // Eagerly load the related Order
                .ToList();

            return Ok(items);
        }

        // ✅ Get a specific inventory item by ID with its linked order
        [HttpGet("{id}")]
        public ActionResult<InventoryItem> GetInventoryItem(int id)
        {
            Console.WriteLine($"GET /api/inventory/{id} was called ✅");

            var item = _context.InventoryItems
                .Include(i => i.Order)
                .FirstOrDefault(i => i.ItemId == id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        // ✅ Create a new inventory item
        [HttpPost]
        public ActionResult<InventoryItem> CreateInventoryItem([FromBody] InventoryItem newItem)
        {
            Console.WriteLine("POST /api/inventory was called ✅");

            _context.InventoryItems.Add(newItem);
            _context.SaveChanges();

            // Return 201 Created with location header pointing to GetInventoryItem
            return CreatedAtAction(nameof(GetInventoryItem), new { id = newItem.ItemId }, newItem);
        }

        // ✅ Update an existing inventory item
        [HttpPut("{id}")]
        public ActionResult UpdateInventoryItem(int id, [FromBody] InventoryItem updatedItem)
        {
            Console.WriteLine($"PUT /api/inventory/{id} was called ✅");

            var existingItem = _context.InventoryItems.Find(id);
            if (existingItem == null)
            {
                return NotFound();
            }

            // Update properties
            existingItem.Name = updatedItem.Name;
            existingItem.Quantity = updatedItem.Quantity;
            existingItem.Location = updatedItem.Location;
            existingItem.OrderId = updatedItem.OrderId;  // Maintain order link

            _context.SaveChanges();

            return NoContent();  // Standard 204 response for successful update
        }

        // ✅ Delete an inventory item by ID
        [HttpDelete("{id}")]
        public ActionResult DeleteInventoryItem(int id)
        {
            Console.WriteLine($"DELETE /api/inventory/{id} was called ✅");

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
