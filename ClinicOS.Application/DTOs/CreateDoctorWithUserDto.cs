using System.ComponentModel.DataAnnotations;

namespace ClinicOS.Application.DTOs;

/// <summary>
/// DTO for creating a new doctor with user account
/// </summary>
public class CreateDoctorWithUserDto
{
    // User fields
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full name is required")]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    // Doctor-specific fields
    [Required(ErrorMessage = "Specialization is required")]
    [StringLength(100)]
    public string Specialization { get; set; } = string.Empty;

    [Required(ErrorMessage = "License number is required")]
    [StringLength(50)]
    public string LicenseNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Years of experience is required")]
    [Range(0, 50, ErrorMessage = "Years of experience must be between 0 and 50")]
    public int YearsOfExperience { get; set; }

    [StringLength(500)]
    public string? Bio { get; set; }

    [Required(ErrorMessage = "Consultation fee is required")]
    [Range(0, 100000, ErrorMessage = "Consultation fee must be between 0 and 100000")]
    public decimal ConsultationFee { get; set; }

    [StringLength(100)]
    public string? Department { get; set; }

    [StringLength(200)]
    public string? ClinicLocation { get; set; }

    public bool IsAvailable { get; set; } = true;

    // ClinicId is optional for regular staff (set from context), but required for super admins
    public int? ClinicId { get; set; }
}
