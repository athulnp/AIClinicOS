using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClinicOS.Domain.Common;

namespace ClinicOS.Domain.Entities;

public class AppointmentNote : AuditableEntity, ITenantEntity
{
    public int ClinicId { get; set; }

    [Required]
    public int AppointmentId { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? CreatedByUser { get; set; }

    [MaxLength(50)]
    public string? NoteType { get; set; } // e.g., "Clinical", "Administrative", "PatientCommunication"

    // Navigation properties
    public Appointment Appointment { get; set; } = null!;
}
