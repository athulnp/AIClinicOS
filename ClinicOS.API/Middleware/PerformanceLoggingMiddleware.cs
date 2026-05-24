using System.Diagnostics;

namespace ClinicOS.API.Middleware;

public class PerformanceLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceLoggingMiddleware> _logger;
    private readonly Stopwatch _stopwatch;

    public PerformanceLoggingMiddleware(RequestDelegate next, ILogger<PerformanceLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _stopwatch = new Stopwatch();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _stopwatch.Start();
        
        try
        {
            await _next(context);
        }
        finally
        {
            _stopwatch.Stop();
            
            var elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
            var requestPath = context.Request.Path;
            var requestMethod = context.Request.Method;
            var statusCode = context.Response.StatusCode;

            if (elapsedMilliseconds > 1000)
            {
                _logger.LogWarning("Slow request detected: {Method} {Path} took {ElapsedMs}ms with status {StatusCode}",
                    requestMethod, requestPath, elapsedMilliseconds, statusCode);
            }
            else
            {
                _logger.LogInformation("Request: {Method} {Path} completed in {ElapsedMs}ms with status {StatusCode}",
                    requestMethod, requestPath, elapsedMilliseconds, statusCode);
            }
        }
    }
}
