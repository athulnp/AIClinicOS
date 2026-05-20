namespace ClinicOS.Application.Interfaces;

/// <summary>
/// Per-request tenant (clinic) context resolved from JWT and optional headers.
/// </summary>
public interface ITenantContext
{
    int? ClinicId { get; }
    string? ClinicCode { get; }
    bool IsSuperAdmin { get; }
    bool HasClinic { get; }
    void SetClinic(int clinicId, string? clinicCode = null);
    void SetSuperAdmin();
}
