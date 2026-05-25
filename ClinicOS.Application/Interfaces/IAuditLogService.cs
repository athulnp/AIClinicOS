using ClinicOS.Domain.Entities;

namespace ClinicOS.Application.Interfaces;

public interface IAuditLogService
{
    Task LogActivityAsync(int clinicId, int userId, string userName, string action, string entityType, int? entityId = null, string? entityName = null, string? description = null, string? ipAddress = null, string? userAgent = null, string? additionalData = null);
    Task<List<AuditLog>> GetRecentActivitiesAsync(int clinicId, int count = 10);
    Task<List<AuditLog>> GetActivitiesByUserAsync(int clinicId, int userId, int count = 20);
}
