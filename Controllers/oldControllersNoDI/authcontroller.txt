using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LogiTrack.Models;
using Microsoft.EntityFrameworkCore;

namespace LogiTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly LogiTrackContext _context;

        public AuthController(LogiTrackContext context)
        {
            _context = context;
        }

        // ✅ Login endpoint
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginData)
        {
            var user = _context.Users.FirstOrDefault(u =>
                u.Username == loginData.Username &&
                u.PasswordHash == loginData.Password // Reminder: Hash properly in real apps
            );

            if (user == null)
                return Unauthorized("Invalid username or password.");

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        // ✅ Register endpoint
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest newUser)
        {
            if (_context.Users.Any(u => u.Username == newUser.Username))
            {
                return BadRequest("Username already exists.");
            }

            var user = new User
            {
                Username = newUser.Username,
                PasswordHash = newUser.Password,  // For demo only — hash properly!
                Role = newUser.Role
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("User registered successfully.");
        }

        // ✅ Token generator — pulls directly from environment variable
        private string GenerateJwtToken(User user)
        {
            var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? throw new Exception("JWT_SECRET is missing from environment variables.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
