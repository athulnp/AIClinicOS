using ClinicOS.Application.Common;
using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Entities;
using ClinicOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Infrastructure.Repositories;

/// <summary>
/// Billing repository implementation
/// </summary>
public class BillingRepository : Repository<Billing>, IBillingRepository
{
    private readonly ITenantContext _tenantContext;

    public BillingRepository(AppDbContext context, ITenantContext tenantContext) : base(context)
    {
        _tenantContext = tenantContext;
    }

    public async Task<Billing?> GetByInvoiceNumberAsync(string invoiceNumber)
    {
        return await _dbSet
            .Include(b => b.Patient)
            .Include(b => b.Appointment)
            .FirstOrDefaultAsync(b => b.InvoiceNumber == invoiceNumber);
    }

    public async Task<IEnumerable<Billing>> GetByPatientIdAsync(int patientId)
    {
        return await _dbSet
            .Include(b => b.Appointment)
            .Where(b => b.PatientId == patientId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Billing>> GetByAppointmentIdAsync(int appointmentId)
    {
        return await _dbSet
            .Include(b => b.Patient)
            .Where(b => b.AppointmentId == appointmentId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Billing>> GetOutstandingBalancesAsync()
    {
        return await _dbSet
            .Include(b => b.Patient)
            .Where(b => b.BalanceAmount > 0)
            .OrderBy(b => b.BalanceAmount)
            .ToListAsync();
    }
public async Task<IEnumerable<Billing>> GetPagedAsync(PaginationRequest pagination, int? clinicId = null)
    {
        IQueryable<Billing> query = _dbSet;

        // Explicitly filter by clinic from TenantContext
        if (_tenantContext.HasClinic)
        {
            query = query.Where(b => b.ClinicId == _tenantContext.ClinicId);
        }

        return await query
            .Include(b => b.Patient)
            .Include(b => b.Appointment)
            .OrderByDescending(b => b.CreatedAt)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalOutstandingForPatientAsync(int patientId)
    {
        return await _dbSet
            .Where(b => b.PatientId == patientId && b.BalanceAmount > 0)
            .SumAsync(b => b.BalanceAmount);
    }
}
