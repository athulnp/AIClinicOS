using ClinicOS.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClinicOS.Application.DTOs;

/// <summary>
/// DTO for creating a new billing record
/// </summary>
public class CreateBillingDto
{
    [Required(ErrorMessage = "Patient ID is required")]
    public int PatientId { get; set; }

    public int? AppointmentId { get; set; }

    [Required(ErrorMessage = "Total amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0")]
    public decimal TotalAmount { get; set; }

    [Required(ErrorMessage = "Payment method is required")]
    public PaymentMethod PaymentMethod { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for updating a billing record
/// </summary>
public class UpdateBillingDto
{
    [Required(ErrorMessage = "Patient ID is required")]
    public int PatientId { get; set; }

    public int? AppointmentId { get; set; }

    [Required(ErrorMessage = "Total amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than 0")]
    public decimal TotalAmount { get; set; }

    [Required(ErrorMessage = "Payment method is required")]
    public PaymentMethod PaymentMethod { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for recording a payment
/// </summary>
public class RecordPaymentDto
{
    [Required(ErrorMessage = "Payment amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Payment amount must be greater than 0")]
    public decimal PaymentAmount { get; set; }

    [Required(ErrorMessage = "Payment method is required")]
    public PaymentMethod PaymentMethod { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for billing response
/// </summary>
public class BillingDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int? AppointmentId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal BalanceAmount { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO for outstanding balance report
/// </summary>
public class OutstandingBalanceReportDto
{
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string PatientPhone { get; set; } = string.Empty;
    public decimal TotalOutstanding { get; set; }
    public int PendingInvoices { get; set; }
}
