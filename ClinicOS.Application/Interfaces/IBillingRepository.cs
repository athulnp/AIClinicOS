using ClinicOS.Application.Common;
using ClinicOS.Domain.Entities;

namespace ClinicOS.Application.Interfaces;

/// <summary>
/// Billing repository interface with specific operations
/// </summary>
public interface IBillingRepository : IRepository<Billing>
{
    Task<Billing?> GetByInvoiceNumberAsync(string invoiceNumber);
    Task<IEnumerable<Billing>> GetByPatientIdAsync(int patientId);
    Task<IEnumerable<Billing>> GetByAppointmentIdAsync(int appointmentId);
    Task<IEnumerable<Billing>> GetOutstandingBalancesAsync();
    Task<IEnumerable<Billing>> GetPagedAsync(PaginationRequest pagination);
    Task<decimal> GetTotalOutstandingForPatientAsync(int patientId);
}
