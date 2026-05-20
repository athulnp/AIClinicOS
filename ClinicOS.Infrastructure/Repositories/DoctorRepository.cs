using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Entities;
using ClinicOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Infrastructure.Repositories;

/// <summary>
/// Doctor repository implementation
/// </summary>
public class DoctorRepository : Repository<Doctor>, IDoctorRepository
{
    public DoctorRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Doctor?> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.UserId == userId);
    }

    public async Task<Doctor?> GetByLicenseNumberAsync(string licenseNumber)
    {
        return await _dbSet
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.LicenseNumber == licenseNumber);
    }

    public async Task<IEnumerable<Doctor>> GetBySpecializationAsync(string specialization)
    {
        return await _dbSet
            .Include(d => d.User)
            .Where(d => d.Specialization == specialization)
            .OrderBy(d => d.User.FullName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Doctor>> GetAvailableDoctorsAsync()
    {
        return await _dbSet
            .Include(d => d.User)
            .Where(d => d.IsAvailable && d.User.IsActive)
            .OrderBy(d => d.User.FullName)
            .ToListAsync();
    }

    public async Task<Doctor> CreateAsync(Doctor doctor)
    {
        await _dbSet.AddAsync(doctor);
        return doctor;
    }

    public async Task<Doctor> UpdateAsync(Doctor doctor)
    {
        _dbSet.Update(doctor);
        return doctor;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var doctor = await _dbSet.FindAsync(id);
        if (doctor == null)
            return false;

        _dbSet.Remove(doctor);
        return true;
    }

    public async Task<bool> LicenseNumberExistsAsync(string licenseNumber, int? excludeId = null)
    {
        var query = _dbSet.Where(d => d.LicenseNumber == licenseNumber);
        if (excludeId.HasValue)
            query = query.Where(d => d.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public override async Task<IEnumerable<Doctor>> GetAllAsync()
    {
        return await _dbSet
            .Include(d => d.User)
            .OrderBy(d => d.User.FullName)
            .ToListAsync();
    }

    public override async Task<Doctor?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == id);
    }
}
