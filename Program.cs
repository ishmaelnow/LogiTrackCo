using LogiTrack;
using LogiTrack.Models;
using LogiTrack.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ✅ Load environment variables from .env file
DotNetEnv.Env.Load();

// ✅ Get sensitive values from environment
var apiKey = Environment.GetEnvironmentVariable("API_KEY") ?? throw new Exception("API_KEY is missing in environment.");
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? throw new Exception("JWT_SECRET is missing in environment.");

// ✅ Register DbContext with DI container
builder.Services.AddDbContext<LogiTrackContext>();

// ✅ Register controllers with JSON options to prevent circular reference crashes
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

// ✅ Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,  // You can set these to true and configure them properly for production
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

// ✅ Add simple Authorization support
builder.Services.AddAuthorization();

var app = builder.Build();

// ✅ Seed database at startup using scoped service
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LogiTrackContext>();
    DbInitializer.Seed(context);
}

// ✅ Add custom API key middleware before hitting endpoints
app.UseMiddleware<ApiKeyMiddleware>();

// ✅ Add authentication and authorization middlewares
app.UseAuthentication();
app.UseAuthorization();

// ✅ Map controller routes (e.g., /api/inventory, /api/order)
app.MapControllers();

app.Run();
