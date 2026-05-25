using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Entities;
using ClinicOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Infrastructure.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext _context;

    public AuditLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AuditLog> AddAsync(AuditLog auditLog)
    {
        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
        return auditLog;
    }

    public async Task<List<AuditLog>> GetRecentActivitiesAsync(int clinicId, int count = 10)
    {
        return await _context.AuditLogs
            .Where(a => a.ClinicId == clinicId)
            .OrderByDescending(a => a.Timestamp)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<AuditLog>> GetActivitiesByUserAsync(int clinicId, int userId, int count = 20)
    {
        return await _context.AuditLogs
            .Where(a => a.ClinicId == clinicId && a.UserId == userId)
            .OrderByDescending(a => a.Timestamp)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<AuditLog>> GetActivitiesByEntityTypeAsync(int clinicId, string entityType, int count = 20)
    {
        return await _context.AuditLogs
            .Where(a => a.ClinicId == clinicId && a.EntityType == entityType)
            .OrderByDescending(a => a.Timestamp)
            .Take(count)
            .ToListAsync();
    }
}
