using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using LogiTrack.Models;

namespace LogiTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // ✅ Require valid JWT for all endpoints
    public class CustomerController : ControllerBase
    {
        private readonly LogiTrackContext _context;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(LogiTrackContext context, ILogger<CustomerController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ✅ GET all customers (Admin only)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<Customer>> GetCustomers()
        {
            _logger.LogInformation("GET /api/customer called ✅");

            var customers = _context.Customers
                .Include(c => c.Orders)
                .ToList();

            return Ok(customers);
        }

        // ✅ GET specific customer by ID (Authenticated users)
        [HttpGet("{id}")]
        public ActionResult<Customer> GetCustomerById(int id)
        {
            _logger.LogInformation($"GET /api/customer/{id} called ✅");

            var customer = _context.Customers
                .Include(c => c.Orders)
                .FirstOrDefault(c => c.CustomerId == id);

            if (customer == null)
            {
                _logger.LogWarning($"Customer with ID {id} not found ❌");
                return NotFound();
            }

            return Ok(customer);
        }

        // ✅ POST create new customer (Admin only)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<Customer> CreateCustomer([FromBody] Customer newCustomer)
        {
            _logger.LogInformation("POST /api/customer called ✅");

            _context.Customers.Add(newCustomer);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetCustomerById), new { id = newCustomer.CustomerId }, newCustomer);
        }

        // ✅ PUT update existing customer (Admin only)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateCustomer(int id, [FromBody] Customer updatedCustomer)
        {
            _logger.LogInformation($"PUT /api/customer/{id} called ✅");

            if (id != updatedCustomer.CustomerId)
            {
                _logger.LogWarning("ID mismatch during update ❌");
                return BadRequest("ID mismatch.");
            }

            var existingCustomer = _context.Customers.Find(id);
            if (existingCustomer == null)
            {
                _logger.LogWarning($"Customer with ID {id} not found ❌");
                return NotFound();
            }

            existingCustomer.Name = updatedCustomer.Name;
            _context.SaveChanges();

            return NoContent();
        }

        // ✅ DELETE customer with safeguard (Admin only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteCustomer(int id)
        {
            _logger.LogInformation($"DELETE /api/customer/{id} called ✅");

            var customer = _context.Customers
                .Include(c => c.Orders)
                .FirstOrDefault(c => c.CustomerId == id);

            if (customer == null)
            {
                _logger.LogWarning($"Customer with ID {id} not found ❌");
                return NotFound();
            }

            if (customer.Orders.Any())
            {
                _logger.LogWarning($"Attempted to delete customer {id} with active orders ❌");
                return BadRequest("Cannot delete customer with existing orders.");
            }

            _context.Customers.Remove(customer);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
