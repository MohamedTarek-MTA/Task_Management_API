using System.Collections.Concurrent;

namespace Task_Management_API.API.Middlewares
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;

        private const int MaxRequestsPerSecond = 10;

        private static readonly ConcurrentDictionary<string, RateLimitInfo>
            Clients = new();

        public RateLimitingMiddleware(
            RequestDelegate next,
            ILogger<RateLimitingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Get client IP
            var clientIp = context.Connection.RemoteIpAddress?.ToString()
                           ?? "unknown";

            var now = DateTime.UtcNow;

            var rateLimitInfo = Clients.GetOrAdd(
                clientIp,
                _ => new RateLimitInfo
                {
                    WindowStart = now,
                    RequestCount = 0
                });

            lock (rateLimitInfo)
            {
                // Start a new one-second window
                if ((now - rateLimitInfo.WindowStart).TotalSeconds >= 10)
                {
                    rateLimitInfo.WindowStart = now;
                    rateLimitInfo.RequestCount = 0;
                }

                rateLimitInfo.RequestCount++;

                if (rateLimitInfo.RequestCount > MaxRequestsPerSecond)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                    context.Response.Headers["Retry-After"] = "1";

                    return;
                }
            }

            await _next(context);
        }

        private class RateLimitInfo
        {
            public DateTime WindowStart { get; set; }

            public int RequestCount { get; set; }
        }
    }
}
