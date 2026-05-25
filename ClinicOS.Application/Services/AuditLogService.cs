using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Entities;

namespace ClinicOS.Application.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _auditLogRepository;

    public AuditLogService(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task LogActivityAsync(int clinicId, int userId, string userName, string action, string entityType, int? entityId = null, string? entityName = null, string? description = null, string? ipAddress = null, string? userAgent = null, string? additionalData = null)
    {
        var auditLog = new AuditLog
        {
            ClinicId = clinicId,
            UserId = userId,
            UserName = userName,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            EntityName = entityName,
            Description = description,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            AdditionalData = additionalData,
            Timestamp = DateTime.UtcNow
        };

        await _auditLogRepository.AddAsync(auditLog);
    }

    public async Task<List<AuditLog>> GetRecentActivitiesAsync(int clinicId, int count = 10)
    {
        return await _auditLogRepository.GetRecentActivitiesAsync(clinicId, count);
    }

    public async Task<List<AuditLog>> GetActivitiesByUserAsync(int clinicId, int userId, int count = 20)
    {
        return await _auditLogRepository.GetActivitiesByUserAsync(clinicId, userId, count);
    }
}
