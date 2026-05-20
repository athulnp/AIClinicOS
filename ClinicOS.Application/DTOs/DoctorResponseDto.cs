namespace ClinicOS.Application.DTOs;

/// <summary>
/// DTO for retrieving doctor details with user information
/// </summary>
public class DoctorResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public string? Bio { get; set; }
    public decimal ConsultationFee { get; set; }
    public string? Department { get; set; }
    public string? ClinicLocation { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
