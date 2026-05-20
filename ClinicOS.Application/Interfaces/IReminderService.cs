using ClinicOS.Application.Common;
using ClinicOS.Application.DTOs;

namespace ClinicOS.Application.Interfaces;

/// <summary>
/// Reminder service interface
/// </summary>
public interface IReminderService
{
    Task SendAppointmentRemindersAsync();
    Task SendFollowUpRemindersAsync();
    Task<ApiResponse<ReminderLogDto>> GetReminderLogsAsync(DateTime? fromDate, DateTime? toDate);
}
