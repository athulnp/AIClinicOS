using System.ComponentModel.DataAnnotations;

namespace ClinicOS.Domain.Entities;

/// <summary>
/// Named role template (SuperAdmin, Admin, Doctor, …). ClinicId null = system template.
/// </summary>
public class Role
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Description { get; set; }

    /// <summary>Null for platform/clinic templates. Set for future per-clinic custom roles.</summary>
    public int? ClinicId { get; set; }

    /// <summary>Platform operator (SuperAdmin) — not assignable via clinic user APIs.</summary>
    public bool IsPlatformRole { get; set; }

    public virtual Clinic? Clinic { get; set; }
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    public ICollection<UserRoleAssignment> UserAssignments { get; set; } = new List<UserRoleAssignment>();
}
