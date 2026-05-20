using ClinicOS.Application.Interfaces;

namespace ClinicOS.Infrastructure.Services;

public class TenantContext : ITenantContext
{
    public int? ClinicId { get; private set; }
    public string? ClinicCode { get; private set; }
    public bool IsSuperAdmin { get; private set; }

    public bool HasClinic => ClinicId.HasValue;

    public void SetClinic(int clinicId, string? clinicCode = null)
    {
        ClinicId = clinicId;
        ClinicCode = clinicCode;
    }

    public void SetSuperAdmin()
    {
        IsSuperAdmin = true;
    }
}
