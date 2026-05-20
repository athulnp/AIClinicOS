using ClinicOS.Application.Common;
using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Entities;
using ClinicOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Infrastructure.Repositories;

public class ClinicRepository : IClinicRepository
{
    private readonly AppDbContext _context;

    public ClinicRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Clinic?> GetByIdAsync(int id) =>
        await _context.Clinics.FindAsync(id);

    public async Task<Clinic?> GetByCodeAsync(string code) =>
        await _context.Clinics.FirstOrDefaultAsync(c => c.Code == code);

    public async Task<IEnumerable<Clinic>> GetAllAsync(bool activeOnly = false)
    {
        var query = _context.Clinics.AsQueryable();
        if (activeOnly)
            query = query.Where(c => c.IsActive);
        return await query.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<IEnumerable<Clinic>> GetPagedAsync(PaginationRequest pagination, bool? isActive = null)
    {
        var query = _context.Clinics.AsQueryable();
        if (isActive.HasValue)
            query = query.Where(c => c.IsActive == isActive.Value);

        return await query
            .OrderBy(c => c.Name)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync(bool? isActive = null)
    {
        var query = _context.Clinics.AsQueryable();
        if (isActive.HasValue)
            query = query.Where(c => c.IsActive == isActive.Value);
        return await query.CountAsync();
    }

    public async Task<bool> CodeExistsAsync(string code) =>
        await _context.Clinics.AnyAsync(c => c.Code == code);

    public async Task AddAsync(Clinic clinic) =>
        await _context.Clinics.AddAsync(clinic);

    public void Update(Clinic clinic) =>
        _context.Clinics.Update(clinic);
}
