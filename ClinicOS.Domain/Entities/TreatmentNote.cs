using ClinicOS.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOS.Domain.Entities;

/// <summary>
/// Treatment note entity representing AI-generated and finalized clinical notes
/// </summary>
public class TreatmentNote : AuditableEntity, ITenantEntity
{
    public int ClinicId { get; set; }

    [Required]
    public int PatientId { get; set; }

    [Required]
    public int AppointmentId { get; set; }

    [Required]
    [MaxLength(200)]
    public string ProcedureType { get; set; } = null!;

    [MaxLength(10)]
    public string? ToothNumber { get; set; }

    [MaxLength(1000)]
    public string? Symptoms { get; set; }

    [MaxLength(1000)]
    public string? Diagnosis { get; set; }

    [MaxLength(2000)]
    public string? TreatmentPerformed { get; set; }

    [MaxLength(1000)]
    public string? AdditionalNotes { get; set; }

    [MaxLength(5000)]
    public string? AiGeneratedNote { get; set; }

    [MaxLength(5000)]
    public string? FinalNote { get; set; }

    [Required]
    public int GeneratedByUserId { get; set; }

    // Navigation properties
    public virtual Clinic Clinic { get; set; } = null!;

    [ForeignKey("PatientId")]
    public virtual Patient Patient { get; set; } = null!;

    [ForeignKey("AppointmentId")]
    public virtual Appointment Appointment { get; set; } = null!;

    [ForeignKey("GeneratedByUserId")]
    public virtual User GeneratedBy { get; set; } = null!;
}
