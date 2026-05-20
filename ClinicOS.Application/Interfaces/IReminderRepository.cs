using ClinicOS.Domain.Entities;

namespace ClinicOS.Application.Interfaces;

/// <summary>
/// Reminder repository interface with specific operations
/// </summary>
public interface IReminderRepository : IRepository<Reminder>
{
    Task<IEnumerable<Reminder>> GetPendingRemindersAsync();
    Task<IEnumerable<Reminder>> GetByAppointmentIdAsync(int appointmentId);
    Task<IEnumerable<Reminder>> GetByStatusAsync(Domain.Enums.ReminderStatus status);
    Task<IEnumerable<Reminder>> GetRemindersDueAsync(DateTime before);
}
