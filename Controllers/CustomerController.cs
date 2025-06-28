using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogiTrack.Models;

namespace LogiTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly LogiTrackContext _context;

        public CustomerController()
        {
            _context = new LogiTrackContext();
        }

        // ✅ Get all customers (optional: include their orders)
        [HttpGet]
        public ActionResult<IEnumerable<Customer>> GetCustomers()
        {
            Console.WriteLine("GET /api/customer was called ✅");

            var customers = _context.Customers
                .Include(c => c.Orders)  // Include related orders if desired
                .ToList();

            return Ok(customers);
        }

        // ✅ Get a specific customer by ID (optional: include their orders)
        [HttpGet("{id}")]
        public ActionResult<Customer> GetCustomerById(int id)
        {
            Console.WriteLine($"GET /api/customer/{id} was called ✅");

            var customer = _context.Customers
                .Include(c => c.Orders)
                .FirstOrDefault(c => c.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        // ✅ Create a new customer
        [HttpPost]
        public ActionResult<Customer> CreateCustomer([FromBody] Customer newCustomer)
        {
            Console.WriteLine("POST /api/customer was called ✅");

            _context.Customers.Add(newCustomer);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetCustomerById), new { id = newCustomer.CustomerId }, newCustomer);
        }

        // ✅ Update an existing customer
        [HttpPut("{id}")]
        public ActionResult UpdateCustomer(int id, [FromBody] Customer updatedCustomer)
        {
            Console.WriteLine($"PUT /api/customer/{id} was called ✅");

            if (id != updatedCustomer.CustomerId)
            {
                return BadRequest("ID mismatch.");
            }

            var existingCustomer = _context.Customers.Find(id);
            if (existingCustomer == null)
            {
                return NotFound();
            }

            existingCustomer.Name = updatedCustomer.Name;

            _context.SaveChanges();

            return NoContent();
        }

        // ✅ Delete a customer by ID
        [HttpDelete("{id}")]
        public ActionResult DeleteCustomer(int id)
        {
            Console.WriteLine($"DELETE /api/customer/{id} was called ✅");

            var customer = _context.Customers
                .Include(c => c.Orders)
                .FirstOrDefault(c => c.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            // Optional: Prevent deletion if they have orders (basic safeguard)
            if (customer.Orders.Any())
            {
                return BadRequest("Cannot delete customer with existing orders.");
            }

            _context.Customers.Remove(customer);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
