using ClinicOS.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOS.Domain.Entities;

/// <summary>
/// Doctor entity representing doctor-specific details
/// </summary>
public class Doctor : AuditableEntity
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Specialization { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string LicenseNumber { get; set; } = string.Empty;

    [Required]
    public int YearsOfExperience { get; set; } = 0;

    [MaxLength(500)]
    public string? Bio { get; set; }

    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal ConsultationFee { get; set; }

    [MaxLength(100)]
    public string? Department { get; set; }

    [MaxLength(200)]
    public string? ClinicLocation { get; set; }

    public bool IsAvailable { get; set; } = true;

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}
