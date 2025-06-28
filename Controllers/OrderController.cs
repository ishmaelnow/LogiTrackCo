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

        public OrderController()
        {
            _context = new LogiTrackContext();
        }

        // ✅ Get all orders with their customer and items
        [HttpGet]
        public ActionResult<IEnumerable<Order>> GetOrders()
        {
            Console.WriteLine("GET /api/order was called ✅");

            var orders = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Items)
                .ToList();

            return Ok(orders);
        }

        // ✅ Get a specific order by ID with customer and items
        [HttpGet("{id}")]
        public ActionResult<Order> GetOrderById(int id)
        {
            Console.WriteLine($"GET /api/order/{id} was called ✅");

            var order = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Items)
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // ✅ Create a new order (linked to a customer by CustomerId)
        [HttpPost]
        public ActionResult<Order> CreateOrder([FromBody] Order newOrder)
        {
            Console.WriteLine("POST /api/order was called ✅");

            // Optional: Verify customer exists before creating order
            var customerExists = _context.Customers.Any(c => c.CustomerId == newOrder.CustomerId);
            if (!customerExists)
            {
                return BadRequest("Invalid CustomerId provided.");
            }

            _context.Orders.Add(newOrder);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetOrderById), new { id = newOrder.OrderId }, newOrder);
        }

        // ✅ Update an existing order
        [HttpPut("{id}")]
        public ActionResult UpdateOrder(int id, [FromBody] Order updatedOrder)
        {
            Console.WriteLine($"PUT /api/order/{id} was called ✅");

            if (id != updatedOrder.OrderId)
            {
                return BadRequest("ID mismatch.");
            }

            var existingOrder = _context.Orders.Find(id);
            if (existingOrder == null)
            {
                return NotFound();
            }

            // Update fields
            existingOrder.DatePlaced = updatedOrder.DatePlaced;
            existingOrder.CustomerId = updatedOrder.CustomerId;

            _context.SaveChanges();

            return NoContent();
        }

        // ✅ Delete an order by ID
        [HttpDelete("{id}")]
        public ActionResult DeleteOrder(int id)
        {
            Console.WriteLine($"DELETE /api/order/{id} was called ✅");

            var order = _context.Orders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
