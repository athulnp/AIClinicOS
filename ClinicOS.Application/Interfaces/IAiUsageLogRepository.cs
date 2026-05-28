using ClinicOS.Domain.Entities;

namespace ClinicOS.Application.Interfaces;

/// <summary>
/// Repository interface for AiUsageLog entity
/// </summary>
public interface IAiUsageLogRepository
{
    Task<AiUsageLog> AddAsync(AiUsageLog aiUsageLog, CancellationToken cancellationToken = default);
    Task<List<AiUsageLog>> GetByClinicIdAsync(int clinicId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<List<AiUsageLog>> GetByUserIdAsync(int userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
