using ClinicOS.Domain.Common;
using ClinicOS.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClinicOS.Domain.Entities;

/// <summary>
/// User entity representing system users (doctors, receptionists, admins)
/// </summary>
public class User : AuditableEntity
{
    /// <summary>Null for platform SuperAdmin users.</summary>
    public int? ClinicId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }

    public bool IsActive { get; set; } = true;

    [MaxLength(500)]
    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    // Navigation properties
    public virtual Clinic? Clinic { get; set; }
    public virtual ICollection<Appointment> DoctorAppointments { get; set; } = new List<Appointment>();
    public virtual Doctor? DoctorDetails { get; set; }
}
