using ClinicOS.Application.Common;
using ClinicOS.Domain.Entities;

namespace ClinicOS.Application.Interfaces;

/// <summary>
/// Patient repository interface with specific operations
/// </summary>
public interface IPatientRepository : ISoftDeleteRepository<Patient>
{
    Task<Patient?> GetByPatientCodeAsync(string patientCode);
    Task<IEnumerable<Patient>> SearchByNameAsync(string name);
    Task<IEnumerable<Patient>> SearchByPhoneAsync(string phone);
    Task<IEnumerable<Patient>> GetPagedAsync(PaginationRequest pagination, int? clinicId = null);
    Task<int> GetTotalCountAsync();
    Task<int> GetNextPatientNumberAsync(int clinicId);
}
