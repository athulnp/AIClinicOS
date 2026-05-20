using ClinicOS.Domain.Enums;

namespace ClinicOS.Application.DTOs;

/// <summary>
/// DTO for reminder response
/// </summary>
public class ReminderDto
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public ReminderType Type { get; set; }
    public DateTime ScheduledFor { get; set; }
    public ReminderStatus Status { get; set; }
    public DateTime? SentAt { get; set; }
    public string? Message { get; set; }
    public string? SentVia { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO for reminder log
/// </summary>
public class ReminderLogDto
{
    public int Id { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public ReminderType Type { get; set; }
    public DateTime ScheduledFor { get; set; }
    public ReminderStatus Status { get; set; }
    public DateTime? SentAt { get; set; }
    public string? Message { get; set; }
    public string? SentVia { get; set; }
}
