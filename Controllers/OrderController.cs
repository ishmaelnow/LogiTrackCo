using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using LogiTrack.Models;

namespace LogiTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // ✅ Require valid JWT for all endpoints
    public class OrderController : ControllerBase
    {
        private readonly LogiTrackContext _context;
        private readonly ILogger<OrderController> _logger;

        public OrderController(LogiTrackContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ✅ GET all orders (Admin only)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<Order>> GetOrders()
        {
            _logger.LogInformation("GET /api/order called ✅");
            var orders = _context.Orders.Include(o => o.Items).ToList();
            return Ok(orders);
        }

        // ✅ GET specific order by ID (Authenticated users)
        [HttpGet("{id}")]
        public ActionResult<Order> GetOrderById(int id)
        {
            _logger.LogInformation($"GET /api/order/{id} called ✅");

            var order = _context.Orders.Include(o => o.Items).FirstOrDefault(o => o.OrderId == id);
            if (order == null)
                return NotFound();

            return Ok(order);
        }

        // ✅ POST create new order (Authenticated users)
        [HttpPost]
        public ActionResult<Order> CreateOrder([FromBody] Order newOrder)
        {
            _logger.LogInformation("POST /api/order called ✅");

            _context.Orders.Add(newOrder);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetOrderById), new { id = newOrder.OrderId }, newOrder);
        }

        // ✅ PUT update order (Admin only)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateOrder(int id, [FromBody] Order updatedOrder)
        {
            _logger.LogInformation($"PUT /api/order/{id} called ✅");

            if (id != updatedOrder.OrderId)
                return BadRequest();

            var existingOrder = _context.Orders.Include(o => o.Items).FirstOrDefault(o => o.OrderId == id);
            if (existingOrder == null)
                return NotFound();

            existingOrder.DatePlaced = updatedOrder.DatePlaced;
            existingOrder.CustomerId = updatedOrder.CustomerId;
            existingOrder.Items = updatedOrder.Items;

            _context.SaveChanges();
            return Ok(existingOrder);
        }

        // ✅ DELETE order (Admin only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteOrder(int id)
        {
            _logger.LogInformation($"DELETE /api/order/{id} called ✅");

            var order = _context.Orders.Include(o => o.Items).FirstOrDefault(o => o.OrderId == id);
            if (order == null)
                return NotFound();

            _context.Orders.Remove(order);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
