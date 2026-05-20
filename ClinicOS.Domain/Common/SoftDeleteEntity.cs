namespace ClinicOS.Domain.Common;

/// <summary>
/// Base entity with soft delete support
/// </summary>
public abstract class SoftDeleteEntity : AuditableEntity, ISoftDelete
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
