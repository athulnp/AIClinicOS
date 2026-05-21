using System.ComponentModel.DataAnnotations;

namespace ClinicOS.Domain.Entities;

/// <summary>
/// Atomic capability (e.g. patients.read). Seeded; codes are stable API contract.
/// </summary>
public class Permission
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? Module { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
