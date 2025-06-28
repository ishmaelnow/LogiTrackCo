using System.ComponentModel.DataAnnotations;

namespace LogiTrack.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }     // Primary key
        public string Name { get; set; } = string.Empty;  // Customer's name

        // Optional: Add more fields like Email, Phone later if desired

        // Navigation property: A customer can have many orders
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
