using Microsoft.AspNetCore.Identity;

namespace LogiTrack.Models
{
    public class ApplicationUser : IdentityUser
    {
        // You can add extra properties here if needed:
        public string? FullName { get; set; }
    }
}
