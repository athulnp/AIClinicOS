using ClinicOS.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ClinicOS.Domain.Entities;

/// <summary>
/// Dental clinic tenant (multi-clinic SaaS).
/// </summary>
public class Clinic : AuditableEntity
{
    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [MaxLength(200)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? State { get; set; }

    [MaxLength(20)]
    public string? PostalCode { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; } = "India";

    public bool IsActive { get; set; } = true;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<Patient> Patients { get; set; } = new List<Patient>();
}
