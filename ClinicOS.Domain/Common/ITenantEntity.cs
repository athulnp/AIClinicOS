namespace ClinicOS.Domain.Common;

/// <summary>
/// Marks an entity as belonging to a dental clinic tenant.
/// </summary>
public interface ITenantEntity
{
    int ClinicId { get; set; }
}
