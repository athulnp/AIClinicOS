using ClinicOS.Application.DTOs;
using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Entities;
using ClinicOS.Domain.Enums;
using ClinicOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public override async Task<User?> GetByIdAsync(int id) =>
        await _dbSet.Include(u => u.Clinic).FirstOrDefaultAsync(u => u.Id == id);

    public async Task<User?> GetByUsernameAsync(string username, int? clinicId)
    {
        return await _dbSet.FirstOrDefaultAsync(u =>
            u.Username == username && u.ClinicId == clinicId);
    }

    public async Task<User?> GetByEmailAsync(string email, int? clinicId)
    {
        return await _dbSet.FirstOrDefaultAsync(u =>
            u.Email == email && u.ClinicId == clinicId);
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _dbSet.FirstOrDefaultAsync(u =>
            u.RefreshToken == refreshToken && u.RefreshTokenExpiryTime > DateTime.UtcNow);
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role, int clinicId) =>
        await _dbSet.Where(u => u.Role == role && u.ClinicId == clinicId)
            .OrderBy(u => u.FullName).ToListAsync();

    public async Task<bool> UsernameExistsAsync(string username, int? clinicId) =>
        await _dbSet.AnyAsync(u => u.Username == username && u.ClinicId == clinicId);

    public async Task<bool> EmailExistsAsync(string email, int? clinicId) =>
        await _dbSet.AnyAsync(u => u.Email == email && u.ClinicId == clinicId);

    public async Task<bool> HasAppointmentsAsync(int userId) =>
        await _context.Appointments.AnyAsync(a => a.DoctorId == userId);

    public async Task<bool> HasDoctorProfileAsync(int userId) =>
        await _context.Doctors.AnyAsync(d => d.UserId == userId);

    public async Task<IEnumerable<User>> GetPagedAsync(int clinicId, UserListQueryDto query)
    {
        var q = BuildListQuery(clinicId, query);
        return await q
            .Include(u => u.Clinic)
            .OrderBy(u => u.FullName)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();
    }

    public async Task<int> GetListCountAsync(int clinicId, UserListQueryDto query) =>
        await BuildListQuery(clinicId, query).CountAsync();

    private IQueryable<User> BuildListQuery(int clinicId, UserListQueryDto query)
    {
        var q = _dbSet.Where(u => u.ClinicId == clinicId);

        if (query.Role.HasValue)
            q = q.Where(u => u.Role == query.Role.Value);

        if (query.IsActive.HasValue)
            q = q.Where(u => u.IsActive == query.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            q = q.Where(u =>
                u.FullName.Contains(term) ||
                u.Username.Contains(term) ||
                u.Email.Contains(term));
        }

        return q;
    }
}
