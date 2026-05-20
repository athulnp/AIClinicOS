namespace ClinicOS.Application.Interfaces;

/// <summary>
/// Unit of Work interface for managing database transactions
/// </summary>
public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}
