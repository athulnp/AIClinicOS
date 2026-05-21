using ClinicOS.Application.Common;
using ClinicOS.Application.Interfaces;
using System.Security.Claims;

namespace ClinicOS.API.Middleware;

/// <summary>
/// Resolves clinic tenant from JWT claims; SuperAdmin may scope via X-Clinic-Id header.
/// </summary>
public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            if (context.User.IsInRole(RoleNames.SuperAdmin))
            {
                tenantContext.SetSuperAdmin();

                if (context.Request.Headers.TryGetValue(TenantConstants.ClinicIdHeader, out var headerValue)
                    && int.TryParse(headerValue.FirstOrDefault(), out var clinicId))
                {
                    tenantContext.SetClinic(clinicId);
                }
            }
            else
            {
                var clinicIdClaim = context.User.FindFirst(TenantConstants.ClinicIdClaim)?.Value;
                if (int.TryParse(clinicIdClaim, out var clinicId))
                {
                    var clinicCode = context.User.FindFirst(TenantConstants.ClinicCodeClaim)?.Value;
                    tenantContext.SetClinic(clinicId, clinicCode);
                }
            }
        }

        await _next(context);
    }
}
