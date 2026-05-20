using ClinicOS.Application.DTOs;

namespace ClinicOS.Application.Interfaces;

/// <summary>
/// Service interface for Doctor operations
/// </summary>
public interface IDoctorService
{
    Task<DoctorResponseDto?> GetDoctorByIdAsync(int id);
    Task<DoctorResponseDto?> GetDoctorByUserIdAsync(int userId);
    Task<DoctorResponseDto?> GetDoctorByLicenseNumberAsync(string licenseNumber);
    Task<IEnumerable<DoctorResponseDto>> GetAllDoctorsAsync();
    Task<IEnumerable<DoctorResponseDto>> GetDoctorsBySpecializationAsync(string specialization);
    Task<IEnumerable<DoctorResponseDto>> GetAvailableDoctorsAsync();
    Task<DoctorResponseDto> CreateDoctorAsync(CreateDoctorDto dto);
    Task<DoctorResponseDto> UpdateDoctorAsync(int id, UpdateDoctorDto dto);
    Task<bool> DeleteDoctorAsync(int id);
}
