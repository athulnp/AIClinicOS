using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Entities;
using ClinicOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for AiUsageLog entity
/// </summary>
public class AiUsageLogRepository : IAiUsageLogRepository
{
    private readonly AppDbContext _context;

    public AiUsageLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AiUsageLog> AddAsync(AiUsageLog aiUsageLog, CancellationToken cancellationToken = default)
    {
        _context.AiUsageLogs.Add(aiUsageLog);
        await _context.SaveChangesAsync(cancellationToken);
        return aiUsageLog;
    }

    public async Task<List<AiUsageLog>> GetByClinicIdAsync(int clinicId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.AiUsageLogs
            .Include(a => a.User)
            .Where(a => a.ClinicId == clinicId)
            .OrderByDescending(a => a.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AiUsageLog>> GetByUserIdAsync(int userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.AiUsageLogs
            .Include(a => a.User)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}
