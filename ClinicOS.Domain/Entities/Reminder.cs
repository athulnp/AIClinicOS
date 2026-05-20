using ClinicOS.Domain.Common;
using ClinicOS.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOS.Domain.Entities;

/// <summary>
/// Reminder entity for appointment and follow-up reminders
/// </summary>
public class Reminder : AuditableEntity
{
    [Required]
    public int AppointmentId { get; set; }

    [Required]
    public ReminderType Type { get; set; }

    [Required]
    public DateTime ScheduledFor { get; set; }

    [Required]
    public ReminderStatus Status { get; set; } = ReminderStatus.Pending;

    public DateTime? SentAt { get; set; }

    [MaxLength(500)]
    public string? Message { get; set; }

    [MaxLength(2000)]
    public string? Response { get; set; }

    [MaxLength(100)]
    public string? SentVia { get; set; } // Future: WhatsApp, SMS, Email

    // Navigation properties
    [ForeignKey("AppointmentId")]
    public virtual Appointment Appointment { get; set; } = null!;
}
