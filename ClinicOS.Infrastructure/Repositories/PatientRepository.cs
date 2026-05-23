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
    private readonly ITenantContext _tenantContext;

    public PatientRepository(AppDbContext context, ITenantContext tenantContext) : base(context)
    {
        _tenantContext = tenantContext;
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

    public async Task<IEnumerable<Patient>> GetPagedAsync(PaginationRequest pagination, int? clinicId = null)
    {
        IQueryable<Patient> query = _dbSet;

        // Explicitly filter by clinic from TenantContext
        if (_tenantContext.HasClinic)
        {
            query = query.Where(p => p.ClinicId == _tenantContext.ClinicId);
        }

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _dbSet.CountAsync();
    }

    public async Task<int> GetNextPatientNumberAsync(int clinicId)
    {
        // Get the highest patient number for this clinic
        // Patient codes are in format P-CLINICCODE-XXX (e.g., P-DEMO-001)
        var patients = await _dbSet
            .Where(p => p.ClinicId == clinicId && p.PatientCode.StartsWith("P-"))
            .ToListAsync();

        if (!patients.Any())
        {
            return 1;
        }

        // Extract the numeric part from the last patient code
        var lastPatient = patients.OrderByDescending(p => p.CreatedAt).First();
        var parts = lastPatient.PatientCode.Split('-');
        if (parts.Length == 3 && int.TryParse(parts[2], out int lastNumber))
        {
            return lastNumber + 1;
        }

        return 1;
    }
}
