using System.ComponentModel.DataAnnotations;

namespace LogiTrack.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }            // Primary key

        [Required]
        public string Username { get; set; } = null!;  // Unique username

        [Required]
        public string PasswordHash { get; set; } = null!;  // Hashed password (never store plain text!)

        [Required]
        public string Role { get; set; } = "User";   // Role can be "User", "Admin", etc.
    }
}
