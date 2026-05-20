using ClinicOS.Application.Common;
using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Entities;
using ClinicOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Infrastructure.Repositories;

/// <summary>
/// Patient repository implementation
/// </summary>
public class PatientRepository : SoftDeleteRepository<Patient>, IPatientRepository
{
    public PatientRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Patient?> GetByPatientCodeAsync(string patientCode)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.PatientCode == patientCode);
    }

    public async Task<IEnumerable<Patient>> SearchByNameAsync(string name)
    {
        return await _dbSet
            .Where(p => p.FullName.Contains(name))
            .ToListAsync();
    }

    public async Task<IEnumerable<Patient>> SearchByPhoneAsync(string phone)
    {
        return await _dbSet
            .Where(p => p.PhoneNumber.Contains(phone))
            .ToListAsync();
    }

    public async Task<IEnumerable<Patient>> GetPagedAsync(PaginationRequest pagination)
    {
        return await _dbSet
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _dbSet.CountAsync();
    }
}
