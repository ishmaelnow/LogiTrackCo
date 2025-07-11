using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using LogiTrack.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;

namespace LogiTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require valid JWT for all order endpoints
    public class OrderController : ControllerBase
    {
        private readonly LogiTrackContext _context;
        private readonly ILogger<OrderController> _logger;

        public OrderController(LogiTrackContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ✅ Get all orders - Admin sees all, customers see their own
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            _logger.LogInformation("GET /api/order called ✅");

            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;

            if (role == "Admin")
            {
                var allOrders = await _context.Orders.Include(o => o.Items).ToListAsync();
                return Ok(allOrders);
            }
            else
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Name == username);

                if (customer == null)
                {
                    _logger.LogWarning($"Customer record not found for user: {username} ❌");
                    return Unauthorized();
                }

                var customerOrders = await _context.Orders
                    .Where(o => o.CustomerId == customer.CustomerId)
                    .Include(o => o.Items)
                    .ToListAsync();

                return Ok(customerOrders);
            }
        }

        // ✅ Get specific order - Admin or owner only
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            _logger.LogInformation($"GET /api/order/{id} called ✅");

            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;

            var order = await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                _logger.LogWarning($"Order with ID {id} not found ❌");
                return NotFound();
            }

            if (role == "Admin")
            {
                return Ok(order);
            }
            else
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Name == username);

                if (customer == null || order.CustomerId != customer.CustomerId)
                {
                    _logger.LogWarning($"Unauthorized access attempt to order {id} by user {username} ❌");
                    return Unauthorized();
                }

                return Ok(order);
            }
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

            var order = await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.OrderId == id);

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
