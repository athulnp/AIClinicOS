using ClinicOS.Domain.Entities;

namespace ClinicOS.Application.Interfaces;

/// <summary>
/// Repository interface for Doctor entity
/// </summary>
public interface IDoctorRepository
{
    Task<Doctor?> GetByIdAsync(int id);
    Task<Doctor?> GetByUserIdAsync(int userId);
    Task<Doctor?> GetByLicenseNumberAsync(string licenseNumber);
    Task<IEnumerable<Doctor>> GetAllAsync();
    Task<IEnumerable<Doctor>> GetBySpecializationAsync(string specialization);
    Task<IEnumerable<Doctor>> GetAvailableDoctorsAsync();
    Task<Doctor> CreateAsync(Doctor doctor);
    Task<Doctor> UpdateAsync(Doctor doctor);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> LicenseNumberExistsAsync(string licenseNumber, int? excludeId = null);
}
