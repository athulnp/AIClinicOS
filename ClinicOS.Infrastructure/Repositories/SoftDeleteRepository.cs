using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Common;
using ClinicOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation with soft delete support
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class SoftDeleteRepository<T> : ISoftDeleteRepository<T> where T : class, ISoftDelete
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public SoftDeleteRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual async Task<int> CountAsync()
    {
        return await _dbSet.CountAsync();
    }

    public virtual async Task<bool> ExistsAsync(int id)
    {
        return await _dbSet.AnyAsync(e => EF.Property<int>(e, "Id") == id);
    }

    public virtual async Task SoftDeleteAsync(int id, string deletedBy)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedBy = deletedBy;
            _dbSet.Update(entity);
        }
    }

    public virtual async Task<IEnumerable<T>> GetAllIncludingDeletedAsync()
    {
        return await _dbSet.IgnoreQueryFilters().ToListAsync();
    }

    public virtual async Task RestoreAsync(int id)
    {
        var entity = await _dbSet.IgnoreQueryFilters().FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        if (entity != null)
        {
            entity.IsDeleted = false;
            entity.DeletedAt = null;
            entity.DeletedBy = null;
            _dbSet.Update(entity);
        }
    }
}
