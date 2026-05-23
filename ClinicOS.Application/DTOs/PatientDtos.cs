using ClinicOS.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClinicOS.Application.DTOs;

/// <summary>
/// DTO for creating a new patient
/// </summary>
public class CreatePatientDto
{
    // PatientCode is auto-generated on the server (P-CLINICCODE-XXX)
    // No need to send it from the client

    [Required(ErrorMessage = "Full name is required")]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gender is required")]
    public Gender Gender { get; set; }

    [Required(ErrorMessage = "Date of birth is required")]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Phone number is required")]
    [MaxLength(20)]
    [Phone(ErrorMessage = "Invalid phone number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(200)]
    [EmailAddress(ErrorMessage = "Invalid email address")]
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

    // ClinicId is optional for regular staff (set from context), but required for super admins
    public int? ClinicId { get; set; }
}

/// <summary>
/// DTO for updating a patient
/// </summary>
public class UpdatePatientDto
{
    [Required(ErrorMessage = "Full name is required")]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gender is required")]
    public Gender Gender { get; set; }

    [Required(ErrorMessage = "Date of birth is required")]
    public DateTime DateOfBirth { get; set; }

    [Required(ErrorMessage = "Phone number is required")]
    [MaxLength(20)]
    [Phone(ErrorMessage = "Invalid phone number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(200)]
    [EmailAddress(ErrorMessage = "Invalid email address")]
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
}

/// <summary>
/// DTO for patient response
/// </summary>
public class PatientDto
{
    public int Id { get; set; }
    public string PatientCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? BloodGroup { get; set; }
    public string? MedicalHistory { get; set; }
    public string? Allergies { get; set; }
    public string? EmergencyContact { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
