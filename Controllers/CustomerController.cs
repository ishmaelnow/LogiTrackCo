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

        // ✅ Get all customers (Admin only)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            _logger.LogInformation("GET /api/customer called ✅");

            var customers = await _context.Customers
                .Include(c => c.Orders)
                .ToListAsync();

            return Ok(customers);
        }

        // ✅ Get specific customer by ID (Authenticated users)
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            _logger.LogInformation($"GET /api/customer/{id} called ✅");

            var customer = await _context.Customers
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
            {
                _logger.LogWarning($"Customer with ID {id} not found ❌");
                return NotFound();
            }

            return Ok(customer);
        }

        // ✅ Create new customer (Admin only)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Customer>> CreateCustomer([FromBody] Customer newCustomer)
        {
            _logger.LogInformation("POST /api/customer called ✅");

            _context.Customers.Add(newCustomer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomerById), new { id = newCustomer.CustomerId }, newCustomer);
        }

        // ✅ Update existing customer (Admin only)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] Customer updatedCustomer)
        {
            _logger.LogInformation($"PUT /api/customer/{id} called ✅");

            if (id != updatedCustomer.CustomerId)
            {
                _logger.LogWarning("ID mismatch during update ❌");
                return BadRequest("ID mismatch.");
            }

            var existingCustomer = await _context.Customers.FindAsync(id);
            if (existingCustomer == null)
            {
                _logger.LogWarning($"Customer with ID {id} not found ❌");
                return NotFound();
            }

            existingCustomer.Name = updatedCustomer.Name;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ✅ Delete customer by ID with safeguard (Admin only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            _logger.LogInformation($"DELETE /api/customer/{id} called ✅");

            var customer = await _context.Customers
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.CustomerId == id);

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
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
