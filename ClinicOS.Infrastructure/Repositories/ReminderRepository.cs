using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Entities;
using ClinicOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Infrastructure.Repositories;

/// <summary>
/// Reminder repository implementation
/// </summary>
public class ReminderRepository : Repository<Reminder>, IReminderRepository
{
    public ReminderRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Reminder>> GetPendingRemindersAsync()
    {
        return await _dbSet
            .Include(r => r.Appointment)
                .ThenInclude(a => a.Patient)
            .Include(r => r.Appointment)
                .ThenInclude(a => a.Doctor)
            .Where(r => r.Status == Domain.Enums.ReminderStatus.Pending)
            .OrderBy(r => r.ScheduledFor)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reminder>> GetByAppointmentIdAsync(int appointmentId)
    {
        return await _dbSet
            .Where(r => r.AppointmentId == appointmentId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reminder>> GetByStatusAsync(Domain.Enums.ReminderStatus status)
    {
        return await _dbSet
            .Include(r => r.Appointment)
                .ThenInclude(a => a.Patient)
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Reminder>> GetRemindersDueAsync(DateTime before)
    {
        return await _dbSet
            .Include(r => r.Appointment)
                .ThenInclude(a => a.Patient)
            .Include(r => r.Appointment)
                .ThenInclude(a => a.Doctor)
            .Where(r => r.Status == Domain.Enums.ReminderStatus.Pending && r.ScheduledFor <= before)
            .OrderBy(r => r.ScheduledFor)
            .ToListAsync();
    }
}
