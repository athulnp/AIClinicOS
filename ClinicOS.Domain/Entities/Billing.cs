using ClinicOS.Domain.Common;
using ClinicOS.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOS.Domain.Entities;

/// <summary>
/// Billing entity representing patient invoices and payments
/// </summary>
public class Billing : AuditableEntity, ITenantEntity
{
    public int ClinicId { get; set; }

    [Required]
    public int PatientId { get; set; }

    public int? AppointmentId { get; set; }

    [Required]
    [MaxLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PaidAmount { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal BalanceAmount { get; set; }

    [Required]
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    [Required]
    public PaymentMethod PaymentMethod { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }

    // Navigation properties
    public virtual Clinic Clinic { get; set; } = null!;

    [ForeignKey("PatientId")]
    public virtual Patient Patient { get; set; } = null!;

    [ForeignKey("AppointmentId")]
    public virtual Appointment? Appointment { get; set; }
}
