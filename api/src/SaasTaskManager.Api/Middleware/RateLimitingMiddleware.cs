using System.Collections.Concurrent;
using System.Net;

namespace SaasTaskManager.Api.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    
    // Store request counts per IP
    private static readonly ConcurrentDictionary<string, RateLimitInfo> _requests = new();
    private const int MaxRequests = 100; // 100 requests per 15 minutes for all endpoints
    private const int TimeWindowMinutes = 15;
    
    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = GetClientIpAddress(context);
        var now = DateTime.UtcNow;
        
        var requestInfo = _requests.GetOrAdd(clientIp, _ => new RateLimitInfo());
        
        lock (requestInfo)
        {
            // Clean old requests (outside the time window)
            requestInfo.RequestTimes.RemoveAll(t => (now - t).TotalMinutes > TimeWindowMinutes);
            
            // Check if we're within limits
            if (requestInfo.RequestTimes.Count >= MaxRequests)
            {
                _logger.LogWarning("Rate limit exceeded for IP {IP}", clientIp);
                
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.Headers.Add("Retry-After", (TimeWindowMinutes * 60).ToString());
                context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
                return;
            }
            
            // Add current request
            requestInfo.RequestTimes.Add(now);
        }

        await _next(context);
    }

    private string GetClientIpAddress(HttpContext context)
    {
        // Check for forwarded headers first (in case of proxy/load balancer)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }


}

public class RateLimitInfo
{
    public List<DateTime> RequestTimes { get; } = new();
} 