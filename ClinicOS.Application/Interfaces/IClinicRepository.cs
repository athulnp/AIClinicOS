using ClinicOS.Application.Common;
using ClinicOS.Domain.Entities;

namespace ClinicOS.Application.Interfaces;

public interface IClinicRepository
{
    Task<Clinic?> GetByIdAsync(int id);
    Task<Clinic?> GetByCodeAsync(string code);
    Task<IEnumerable<Clinic>> GetAllAsync(bool activeOnly = false);
    Task<IEnumerable<Clinic>> GetPagedAsync(PaginationRequest pagination, bool? isActive = null);
    Task<int> GetTotalCountAsync(bool? isActive = null);
    Task<bool> CodeExistsAsync(string code);
    Task AddAsync(Clinic clinic);
    void Update(Clinic clinic);
}
