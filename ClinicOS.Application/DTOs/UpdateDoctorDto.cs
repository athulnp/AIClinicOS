namespace ClinicOS.Application.DTOs;

/// <summary>
/// DTO for updating doctor details
/// </summary>
public class UpdateDoctorDto
{
    public string? Specialization { get; set; }
    public string? LicenseNumber { get; set; }
    public int? YearsOfExperience { get; set; }
    public string? Bio { get; set; }
    public decimal? ConsultationFee { get; set; }
    public string? Department { get; set; }
    public string? ClinicLocation { get; set; }
    public bool? IsAvailable { get; set; }
}
