using LogiTrack.Models;       // Access to your entity models
using LogiTrack;              // Access to DbInitializer
using LogiTrack.Middleware;   // Access to ApiKeyMiddleware

var builder = WebApplication.CreateBuilder(args);

// ✅ Adds controller support AND prevents circular reference crash
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

var app = builder.Build();

// ✅ Seed test data at startup
using (var context = new LogiTrackContext())
{
    DbInitializer.Seed(context);
}

// ✅ Insert API Key Middleware to protect non-GET routes
app.UseMiddleware<ApiKeyMiddleware>();

app.MapControllers();   // Maps your routes like /api/inventory or /api/order

app.Run();
