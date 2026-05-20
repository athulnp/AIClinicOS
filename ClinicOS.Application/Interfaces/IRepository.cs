using ClinicOS.Application.Common;
using ClinicOS.Domain.Common;

namespace ClinicOS.Application.Interfaces;

/// <summary>
/// Generic repository interface with common CRUD operations
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    void Update(T entity);
    void Delete(T entity);
    Task<int> CountAsync();
    Task<bool> ExistsAsync(int id);
}

/// <summary>
/// Generic repository interface with soft delete support
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface ISoftDeleteRepository<T> where T : class, ISoftDelete
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    void Update(T entity);
    void Delete(T entity);
    Task<int> CountAsync();
    Task<bool> ExistsAsync(int id);
    Task SoftDeleteAsync(int id, string deletedBy);
    Task<IEnumerable<T>> GetAllIncludingDeletedAsync();
    Task RestoreAsync(int id);
}
