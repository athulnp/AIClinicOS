using ClinicOS.Domain.Common;
using ClinicOS.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOS.Domain.Entities;

/// <summary>
/// Appointment entity representing scheduled appointments
/// </summary>
public class Appointment : AuditableEntity, ITenantEntity
{
    public int ClinicId { get; set; }

    [Required]
    public int PatientId { get; set; }

    [Required]
    public int DoctorId { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    [Required]
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

    [MaxLength(500)]
    public string? Reason { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }

    // Navigation properties
    public virtual Clinic Clinic { get; set; } = null!;

    [ForeignKey("PatientId")]
    public virtual Patient Patient { get; set; } = null!;

    [ForeignKey("DoctorId")]
    public virtual User Doctor { get; set; } = null!;

    public virtual ICollection<Billing> Billings { get; set; } = new List<Billing>();
    public virtual ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
}
