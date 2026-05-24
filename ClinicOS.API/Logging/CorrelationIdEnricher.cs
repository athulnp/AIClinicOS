using Microsoft.AspNetCore.Http;
using Serilog.Core;
using Serilog.Events;

namespace ClinicOS.API.Logging;

public class CorrelationIdEnricher : ILogEventEnricher
{
    private static IHttpContextAccessor _httpContextAccessor;

    public CorrelationIdEnricher()
    {
    }

    public static void SetHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = _httpContextAccessor?.HttpContext;
        
        if (httpContext != null && httpContext.Items.TryGetValue("CorrelationId", out var correlationId) &&
            correlationId != null)
        {
            var correlationIdProperty = propertyFactory.CreateProperty("CorrelationId", correlationId.ToString());
            logEvent.AddPropertyIfAbsent(correlationIdProperty);
        }
    }
}
