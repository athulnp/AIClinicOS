using ClinicOS.Application.Common;
using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Entities;
using ClinicOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Infrastructure.Repositories;

/// <summary>
/// Appointment repository implementation
/// </summary>
public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
{
    private readonly ITenantContext _tenantContext;

    public AppointmentRepository(AppDbContext context, ITenantContext tenantContext) : base(context)
    {
        _tenantContext = tenantContext;
    }

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId)
    {
        return await _dbSet
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctorId)
            .OrderBy(a => a.AppointmentDate)
            .ThenBy(a => a.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => a.AppointmentDate >= startDate && a.AppointmentDate <= endDate)
            .OrderBy(a => a.AppointmentDate)
            .ThenBy(a => a.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetDoctorScheduleAsync(int doctorId, DateTime date)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date == date.Date)
            .OrderBy(a => a.StartTime)
            .ToListAsync();
    }

    public async Task<bool> HasOverlappingAppointmentAsync(int doctorId, DateTime date, TimeSpan startTime, TimeSpan endTime, int? excludeAppointmentId = null)
    {
        var query = _dbSet.Where(a =>
            a.DoctorId == doctorId &&
            a.AppointmentDate.Date == date.Date &&
            a.Status != Domain.Enums.AppointmentStatus.Cancelled &&
            a.Status != Domain.Enums.AppointmentStatus.NoShow &&
            ((a.StartTime < endTime && a.EndTime > startTime) || // Overlap check
             (a.StartTime >= startTime && a.StartTime < endTime) ||
             (a.EndTime > startTime && a.EndTime <= endTime) ||
             (a.StartTime <= startTime && a.EndTime >= endTime)));

        if (excludeAppointmentId.HasValue)
        {
            query = query.Where(a => a.Id != excludeAppointmentId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<Appointment>> GetPagedAsync(PaginationRequest pagination, int? clinicId = null)
    {
        IQueryable<Appointment> query = _dbSet;

        // Explicitly filter by clinic from TenantContext
        if (_tenantContext.HasClinic)
        {
            query = query.Where(a => a.ClinicId == _tenantContext.ClinicId);
        }

        return await query
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .OrderByDescending(a => a.AppointmentDate)
            .ThenBy(a => a.StartTime)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();
    }
}
