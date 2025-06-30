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
    [Authorize] // ✅ Require valid JWT for all order endpoints
    public class OrderController : ControllerBase
    {
        private readonly LogiTrackContext _context;
        private readonly ILogger<OrderController> _logger;

        public OrderController(LogiTrackContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ✅ Get all orders (Admin only)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            _logger.LogInformation("GET /api/order called ✅");

            var orders = await _context.Orders
                .Include(o => o.Items)
                .ToListAsync();

            return Ok(orders);
        }

        // ✅ Get specific order by ID (Authenticated users)
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            _logger.LogInformation($"GET /api/order/{id} called ✅");

            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                _logger.LogWarning($"Order with ID {id} not found ❌");
                return NotFound();
            }

            return Ok(order);
        }

        // ✅ Create new order (Authenticated users)
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] Order newOrder)
        {
            _logger.LogInformation("POST /api/order called ✅");

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrderById), new { id = newOrder.OrderId }, newOrder);
        }

        // ✅ Delete order by ID (Admin only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            _logger.LogInformation($"DELETE /api/order/{id} called ✅");

            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                _logger.LogWarning($"Order with ID {id} not found ❌");
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
