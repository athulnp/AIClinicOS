namespace ClinicOS.Application.DTOs;

/// <summary>
/// DTO for creating a new doctor
/// </summary>
public class CreateDoctorDto
{
    public int UserId { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public string? Bio { get; set; }
    public decimal ConsultationFee { get; set; }
    public string? Department { get; set; }
    public string? ClinicLocation { get; set; }
    public bool IsAvailable { get; set; } = true;

    // ClinicId is optional for regular staff (set from context), but required for super admins
    public int? ClinicId { get; set; }
}
