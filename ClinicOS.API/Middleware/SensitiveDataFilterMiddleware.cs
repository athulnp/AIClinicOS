namespace ClinicOS.API.Middleware;

public class SensitiveDataFilterMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SensitiveDataFilterMiddleware> _logger;

    private static readonly string[] SensitiveFields = 
    {
        "password", "token", "creditcard", "ssn", "socialsecurity",
        "apikey", "secret", "authorization", "cookie"
    };

    public SensitiveDataFilterMiddleware(RequestDelegate next, ILogger<SensitiveDataFilterMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Enable request/response body buffering for logging
        context.Request.EnableBuffering();
        
        var originalBodyStream = context.Response.Body;
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        try
        {
            await _next(context);
        }
        finally
        {
            memoryStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
            
            // Filter sensitive data from response body if it's JSON
            if (!string.IsNullOrEmpty(responseBody) && IsJson(responseBody))
            {
                var filteredBody = FilterSensitiveData(responseBody);
                if (filteredBody != responseBody)
                {
                    _logger.LogDebug("Sensitive data filtered from response body");
                }
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            await memoryStream.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }
    }

    private bool IsJson(string text)
    {
        return text.TrimStart().StartsWith('{') || text.TrimStart().StartsWith('[');
    }

    private string FilterSensitiveData(string json)
    {
        var filtered = json;
        
        foreach (var field in SensitiveFields)
        {
            // Simple regex to mask sensitive field values
            var pattern = $"\"{field}\"\\s*:\\s*\"[^\"]*\"";
            filtered = System.Text.RegularExpressions.Regex.Replace(
                filtered, 
                pattern, 
                $"\"{field}\": \"***MASKED***\"", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        return filtered;
    }
}
