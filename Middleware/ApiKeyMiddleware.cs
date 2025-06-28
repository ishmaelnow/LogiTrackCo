using Microsoft.AspNetCore.Http;

namespace LogiTrack.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string ApiKeyHeaderName = "X-API-KEY";
        private const string ValidApiKey = "supersecretkey123";  // Replace with your secure key

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only protect non-GET requests (POST, PUT, DELETE)
            if (context.Request.Method != HttpMethods.Get)
            {
                if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    await context.Response.WriteAsync("API Key missing.");
                    return;
                }

                if (!string.Equals(extractedApiKey, ValidApiKey))
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    await context.Response.WriteAsync("Invalid API Key.");
                    return;
                }
            }

            await _next(context); // Pass request to next middleware
        }
    }
}
