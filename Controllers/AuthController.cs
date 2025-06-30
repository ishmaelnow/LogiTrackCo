using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LogiTrack.Models;

namespace LogiTrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        // ✅ Register endpoint with Identity flow and role assignment
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest newUser)
        {
            var user = new ApplicationUser
            {
                UserName = newUser.Username
            };

            var result = await _userManager.CreateAsync(user, newUser.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Optional: Assign role if provided
            if (!string.IsNullOrEmpty(newUser.Role))
            {
                await _userManager.AddToRoleAsync(user, newUser.Role);
            }

            return Ok("User registered successfully.");
        }

        // ✅ Login endpoint with Identity password check and JWT generation
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginData)
        {
            var user = await _userManager.FindByNameAsync(loginData.Username);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginData.Password))
            {
                return Unauthorized("Invalid username or password.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user.UserName, roles.FirstOrDefault() ?? "User");

            return Ok(new { token });
        }

        // ✅ JWT Token Generator
        private string GenerateJwtToken(string username, string role)
        {
            var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? throw new Exception("JWT_SECRET is missing from environment variables.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
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
