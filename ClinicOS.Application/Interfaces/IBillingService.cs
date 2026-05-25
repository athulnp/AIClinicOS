using ClinicOS.Application.Common;
using ClinicOS.Application.DTOs;

namespace ClinicOS.Application.Interfaces;

/// <summary>
/// Billing service interface
/// </summary>
public interface IBillingService
{
    Task<ApiResponse<BillingDto>> CreateBillingAsync(CreateBillingDto dto, string createdBy, int userId, int? clinicId = null);
    Task<ApiResponse<BillingDto>> UpdateBillingAsync(int id, UpdateBillingDto dto, string updatedBy, int userId);
    Task<ApiResponse> DeleteBillingAsync(int id, string deletedBy, int userId);
    Task<ApiResponse<BillingDto>> RecordPaymentAsync(int id, RecordPaymentDto dto, string updatedBy, int userId);
    Task<ApiResponse<BillingDto>> GetBillingByIdAsync(int id);
    Task<ApiResponse<BillingDto>> GetBillingByInvoiceNumberAsync(string invoiceNumber);
    Task<PagedResponse<BillingDto>> GetPatientBillingHistoryAsync(int patientId, PaginationRequest pagination);
    Task<PagedResponse<BillingDto>> GetAllBillingsAsync(PaginationRequest pagination, int? clinicId = null);
    Task<PagedResponse<OutstandingBalanceReportDto>> GetOutstandingBalanceReportAsync(PaginationRequest pagination);
}
