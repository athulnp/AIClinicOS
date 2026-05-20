using ClinicOS.Application.Interfaces;

namespace ClinicOS.Infrastructure.Data;

/// <summary>
/// Tenant context used at EF design-time (migrations) — no clinic filter applied.
/// </summary>
internal sealed class DesignTimeTenantContext : ITenantContext
{
    public int? ClinicId => null;
    public string? ClinicCode => null;
    public bool IsSuperAdmin => false;
    public bool HasClinic => false;

    public void SetClinic(int clinicId, string? clinicCode = null) { }
    public void SetSuperAdmin() { }
}
