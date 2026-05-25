using ClinicOS.Domain.Entities;

namespace ClinicOS.Application.Interfaces;

public interface IAuditLogRepository
{
    Task<AuditLog> AddAsync(AuditLog auditLog);
    Task<List<AuditLog>> GetRecentActivitiesAsync(int clinicId, int count = 10);
    Task<List<AuditLog>> GetActivitiesByUserAsync(int clinicId, int userId, int count = 20);
    Task<List<AuditLog>> GetActivitiesByEntityTypeAsync(int clinicId, string entityType, int count = 20);
}
