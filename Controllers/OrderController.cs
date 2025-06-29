using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogiTrack.Models;

namespace LogiTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly LogiTrackContext _context;
        private readonly ILogger<OrderController> _logger;

        public OrderController(LogiTrackContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/order
        [HttpGet]
        public ActionResult<IEnumerable<Order>> GetOrders()
        {
            _logger.LogInformation("GET /api/order called ✅");
            // Retrieve all orders including their items
            var orders = _context.Orders.Include(o => o.Items).ToList();
            return Ok(orders);
        }

        // GET: api/order/{id}
        [HttpGet("{id}")]
        public ActionResult<Order> GetOrderById(int id)
        {
            _logger.LogInformation($"GET /api/order/{id} called ✅");
            // Find order by id including its items
            var order = _context.Orders.Include(o => o.Items).FirstOrDefault(o => o.OrderId == id);
            if (order == null) return NotFound(); // Return 404 if not found
            return Ok(order);
        }

        // POST: api/order
        [HttpPost]
        public ActionResult<Order> CreateOrder([FromBody] Order newOrder)
        {
            _logger.LogInformation("POST /api/order called ✅");
            // Add new order to the database
            _context.Orders.Add(newOrder);
            _context.SaveChanges();
            // Return 201 Created with the new order's location
            return CreatedAtAction(nameof(GetOrderById), new { id = newOrder.OrderId }, newOrder);
        }

        // PUT: api/order/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateOrder(int id, [FromBody] Order updatedOrder)
        {
            _logger.LogInformation($"PUT /api/order/{id} called ✅");
            if (id != updatedOrder.OrderId) return BadRequest(); // Ensure id matches
            // Find existing order including its items
            var existingOrder = _context.Orders.Include(o => o.Items).FirstOrDefault(o => o.OrderId == id);
            if (existingOrder == null) return NotFound(); // Return 404 if not found
            // Update order properties
            existingOrder.DatePlaced = updatedOrder.DatePlaced;
            existingOrder.CustomerId = updatedOrder.CustomerId;
            existingOrder.Items = updatedOrder.Items;
            _context.SaveChanges();
            return Ok(existingOrder);
        }

        // DELETE: api/order/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id)
        {
            _logger.LogInformation($"DELETE /api/order/{id} called ✅");
            // Find order by id including its items
            var order = _context.Orders.Include(o => o.Items).FirstOrDefault(o => o.OrderId == id);
            if (order == null) return NotFound(); // Return 404 if not found
            // Remove order from the database
            _context.Orders.Remove(order);
            _context.SaveChanges();
            return NoContent(); // Return 204 No Content
        }
    }
}
