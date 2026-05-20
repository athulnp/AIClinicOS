using ClinicOS.Domain.Common;
using ClinicOS.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOS.Domain.Entities;

/// <summary>
/// Patient entity representing a patient in the clinic
/// </summary>
public class Patient : SoftDeleteEntity
{
    [Required]
    [MaxLength(50)]
    public string PatientCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public Gender Gender { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [MaxLength(20)]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(200)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(10)]
    public string? BloodGroup { get; set; }

    [MaxLength(2000)]
    public string? MedicalHistory { get; set; }

    [MaxLength(1000)]
    public string? Allergies { get; set; }

    [MaxLength(200)]
    public string? EmergencyContact { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }

    // Navigation properties
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public virtual ICollection<Billing> Billings { get; set; } = new List<Billing>();
}
