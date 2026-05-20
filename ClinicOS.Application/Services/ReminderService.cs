using ClinicOS.Application.Common;
using ClinicOS.Application.DTOs;
using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Entities;
using ClinicOS.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace ClinicOS.Application.Services;

/// <summary>
/// Reminder service implementation
/// </summary>
public class ReminderService : IReminderService
{
    private readonly IReminderRepository _reminderRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ILogger<ReminderService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ReminderService(
        IReminderRepository reminderRepository,
        IAppointmentRepository appointmentRepository,
        ILogger<ReminderService> logger,
        IUnitOfWork unitOfWork)
    {
        _reminderRepository = reminderRepository;
        _appointmentRepository = appointmentRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task SendAppointmentRemindersAsync()
    {
        try
        {
            // Get appointments scheduled for tomorrow
            var tomorrow = DateTime.UtcNow.AddDays(1).Date;
            var appointments = await _appointmentRepository.GetByDateRangeAsync(tomorrow, tomorrow.AddDays(1).AddTicks(-1));

            foreach (var appointment in appointments)
            {
                // Check if reminder already exists
                var existingReminders = await _reminderRepository.GetByAppointmentIdAsync(appointment.Id);
                if (existingReminders.Any(r => r.Type == ReminderType.AppointmentReminder && r.Status == ReminderStatus.Sent))
                {
                    continue;
                }

                // Create reminder
                var reminder = new Reminder
                {
                    AppointmentId = appointment.Id,
                    Type = ReminderType.AppointmentReminder,
                    ScheduledFor = DateTime.UtcNow,
                    Status = ReminderStatus.Pending,
                    Message = $"Reminder: You have an appointment on {appointment.AppointmentDate:yyyy-MM-dd} at {appointment.StartTime} with Dr. {appointment.Doctor.FullName}"
                };

                await _reminderRepository.AddAsync(reminder);
                await _unitOfWork.SaveChangesAsync();

                // Simulate sending reminder (in production, integrate with WhatsApp/SMS)
                await SendReminderAsync(reminder);
            }

            _logger.LogInformation($"Processed {appointments.Count()} appointment reminders");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending appointment reminders");
        }
    }

    public async Task SendFollowUpRemindersAsync()
    {
        try
        {
            // Get completed appointments from 3 days ago
            var threeDaysAgo = DateTime.UtcNow.AddDays(-3).Date;
            var appointments = await _appointmentRepository.GetByDateRangeAsync(threeDaysAgo, threeDaysAgo.AddDays(1).AddTicks(-1));

            foreach (var appointment in appointments.Where(a => a.Status == AppointmentStatus.Completed))
            {
                // Check if follow-up reminder already exists
                var existingReminders = await _reminderRepository.GetByAppointmentIdAsync(appointment.Id);
                if (existingReminders.Any(r => r.Type == ReminderType.FollowUpReminder))
                {
                    continue;
                }

                // Create follow-up reminder
                var reminder = new Reminder
                {
                    AppointmentId = appointment.Id,
                    Type = ReminderType.FollowUpReminder,
                    ScheduledFor = DateTime.UtcNow,
                    Status = ReminderStatus.Pending,
                    Message = $"Follow-up: How was your appointment on {appointment.AppointmentDate:yyyy-MM-dd}? Please let us know if you have any concerns."
                };

                await _reminderRepository.AddAsync(reminder);
                await _unitOfWork.SaveChangesAsync();

                // Simulate sending reminder
                await SendReminderAsync(reminder);
            }

            _logger.LogInformation($"Processed follow-up reminders");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending follow-up reminders");
        }
    }

    public async Task<ApiResponse<ReminderLogDto>> GetReminderLogsAsync(DateTime? fromDate, DateTime? toDate)
    {
        try
        {
            var reminders = await _reminderRepository.GetByStatusAsync(ReminderStatus.Sent);

            if (fromDate.HasValue)
            {
                reminders = reminders.Where(r => r.SentAt >= fromDate.Value).ToList();
            }

            if (toDate.HasValue)
            {
                reminders = reminders.Where(r => r.SentAt <= toDate.Value).ToList();
            }

            var logs = reminders.Select(r => new ReminderLogDto
            {
                Id = r.Id,
                PatientName = r.Appointment.Patient.FullName,
                DoctorName = r.Appointment.Doctor.FullName,
                AppointmentDate = r.Appointment.AppointmentDate,
                Type = r.Type,
                ScheduledFor = r.ScheduledFor,
                Status = r.Status,
                SentAt = r.SentAt,
                Message = r.Message,
                SentVia = r.SentVia
            }).ToList();

            return ApiResponse<ReminderLogDto>.SuccessResponse(logs.FirstOrDefault()!, $"Retrieved {logs.Count} reminder logs");
        }
        catch (Exception ex)
        {
            return ApiResponse<ReminderLogDto>.ErrorResponse("Error retrieving reminder logs");
        }
    }

    private async Task SendReminderAsync(Reminder reminder)
    {
        try
        {
            // Simulate sending reminder (log the action)
            _logger.LogInformation($"Sending reminder: {reminder.Message} to patient: {reminder.Appointment.Patient.FullName}");

            // In production, integrate with WhatsApp/SMS APIs here
            // For now, we'll just mark as sent with a simulated channel
            reminder.Status = ReminderStatus.Sent;
            reminder.SentAt = DateTime.UtcNow;
            reminder.SentVia = "Log"; // Future: WhatsApp, SMS, Email
            reminder.Response = "Reminder logged successfully";

            _reminderRepository.Update(reminder);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error sending reminder for appointment {reminder.AppointmentId}");
            
            reminder.Status = ReminderStatus.Failed;
            reminder.Response = ex.Message;
            _reminderRepository.Update(reminder);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
