using AutoMapper;
using ClinicOS.Application.Common;
using ClinicOS.Application.DTOs;
using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Entities;
using ClinicOS.Domain.Enums;
using FluentValidation;

namespace ClinicOS.Application.Services;

/// <summary>
/// Billing service implementation
/// </summary>
public class BillingService : IBillingService
{
    private readonly IBillingRepository _billingRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IValidator<CreateBillingDto> _createValidator;
    private readonly IValidator<UpdateBillingDto> _updateValidator;
    private readonly IValidator<RecordPaymentDto> _paymentValidator;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogService _auditLogService;

    public BillingService(
        IBillingRepository billingRepository,
        IPatientRepository patientRepository,
        IAppointmentRepository appointmentRepository,
        IValidator<CreateBillingDto> createValidator,
        IValidator<UpdateBillingDto> updateValidator,
        IValidator<RecordPaymentDto> paymentValidator,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IAuditLogService auditLogService)
    {
        _billingRepository = billingRepository;
        _patientRepository = patientRepository;
        _appointmentRepository = appointmentRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _paymentValidator = paymentValidator;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _auditLogService = auditLogService;
    }

    public async Task<ApiResponse<BillingDto>> CreateBillingAsync(CreateBillingDto dto, string createdBy, int userId, int? clinicId = null)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return ApiResponse<BillingDto>.ErrorResponse("Validation failed", validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        // Check if patient exists
        var patient = await _patientRepository.GetByIdAsync(dto.PatientId);
        if (patient == null)
        {
            return ApiResponse<BillingDto>.ErrorResponse("Patient not found");
        }

        // Generate invoice number
        var invoiceNumber = await GenerateInvoiceNumberAsync();

        var billing = _mapper.Map<Billing>(dto);
        // Use clinicId from parameter if provided (for super admins), otherwise use patient's clinic
        billing.ClinicId = clinicId ?? patient.ClinicId;
        billing.InvoiceNumber = invoiceNumber;
        billing.PaidAmount = 0;
        billing.BalanceAmount = dto.TotalAmount;
        billing.PaymentStatus = PaymentStatus.Pending;
        billing.CreatedBy = createdBy;

        await _billingRepository.AddAsync(billing);
        await _unitOfWork.SaveChangesAsync();

        // Log audit activity
        if (billing.ClinicId > 0)
        {
            await _auditLogService.LogActivityAsync(
                billing.ClinicId,
                userId,
                createdBy,
                "CREATE",
                "Billing",
                billing.Id,
                $"Invoice {billing.InvoiceNumber}",
                $"Created billing record for patient {patient.FullName} with amount {dto.TotalAmount}"
            );
        }

        var billingDto = await MapToDto(billing);
        return ApiResponse<BillingDto>.SuccessResponse(billingDto, "Billing record created successfully");
    }

    public async Task<ApiResponse<BillingDto>> UpdateBillingAsync(int id, UpdateBillingDto dto, string updatedBy, int userId)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return ApiResponse<BillingDto>.ErrorResponse("Validation failed", validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var billing = await _billingRepository.GetByIdAsync(id);
        if (billing == null)
        {
            return ApiResponse<BillingDto>.ErrorResponse("Billing record not found");
        }

        billing.PatientId = dto.PatientId;
        billing.TotalAmount = dto.TotalAmount;
        billing.PaymentMethod = dto.PaymentMethod;
        billing.Notes = dto.Notes;
        billing.BalanceAmount = dto.TotalAmount - billing.PaidAmount;
        billing.UpdatedBy = updatedBy;

        _billingRepository.Update(billing);
        await _unitOfWork.SaveChangesAsync();

        // Log audit activity
        if (billing.ClinicId > 0)
        {
            await _auditLogService.LogActivityAsync(
                billing.ClinicId,
                userId,
                updatedBy,
                "UPDATE",
                "Billing",
                billing.Id,
                $"Invoice {billing.InvoiceNumber}",
                $"Updated billing record with amount {dto.TotalAmount}"
            );
        }

        var billingDto = await MapToDto(billing);
        return ApiResponse<BillingDto>.SuccessResponse(billingDto, "Billing record updated successfully");
    }

    public async Task<ApiResponse> DeleteBillingAsync(int id, string deletedBy, int userId)
    {
        var billing = await _billingRepository.GetByIdAsync(id);
        if (billing == null)
        {
            return ApiResponse.ErrorResponse("Billing record not found");
        }

        _billingRepository.Delete(billing);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse.SuccessResponse("Billing record deleted successfully");
    }

    public async Task<ApiResponse<BillingDto>> RecordPaymentAsync(int id, RecordPaymentDto dto, string updatedBy, int userId)
    {
        var validationResult = await _paymentValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return ApiResponse<BillingDto>.ErrorResponse("Validation failed", validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var billing = await _billingRepository.GetByIdAsync(id);
        if (billing == null)
        {
            return ApiResponse<BillingDto>.ErrorResponse("Billing record not found");
        }

        billing.PaidAmount += dto.PaymentAmount;
        billing.BalanceAmount = billing.TotalAmount - billing.PaidAmount;
        billing.PaymentMethod = dto.PaymentMethod;
        billing.UpdatedBy = updatedBy;

        // Update payment status
        if (billing.BalanceAmount <= 0)
        {
            billing.PaymentStatus = PaymentStatus.Paid;
            billing.BalanceAmount = 0;
        }
        else if (billing.PaidAmount > 0)
        {
            billing.PaymentStatus = PaymentStatus.Partial;
        }

        _billingRepository.Update(billing);
        await _unitOfWork.SaveChangesAsync();

        // Log audit activity
        if (billing.ClinicId > 0)
        {
            await _auditLogService.LogActivityAsync(
                billing.ClinicId,
                userId,
                updatedBy,
                "PAYMENT_RECORDED",
                "Billing",
                billing.Id,
                $"Invoice {billing.InvoiceNumber}",
                $"Recorded payment of {dto.PaymentAmount} via {dto.PaymentMethod}. Balance: {billing.BalanceAmount}"
            );
        }

        var billingDto = await MapToDto(billing);
        return ApiResponse<BillingDto>.SuccessResponse(billingDto, "Payment recorded successfully");
    }

    public async Task<ApiResponse<BillingDto>> GetBillingByIdAsync(int id)
    {
        var billing = await _billingRepository.GetByIdAsync(id);
        if (billing == null)
        {
            return ApiResponse<BillingDto>.ErrorResponse("Billing record not found");
        }

        var billingDto = await MapToDto(billing);
        return ApiResponse<BillingDto>.SuccessResponse(billingDto);
    }

    public async Task<ApiResponse<BillingDto>> GetBillingByInvoiceNumberAsync(string invoiceNumber)
    {
        var billing = await _billingRepository.GetByInvoiceNumberAsync(invoiceNumber);
        if (billing == null)
        {
            return ApiResponse<BillingDto>.ErrorResponse("Billing record not found");
        }

        var billingDto = _mapper.Map<BillingDto>(billing);
        return ApiResponse<BillingDto>.SuccessResponse(billingDto);
    }

    public async Task<PagedResponse<BillingDto>> GetPatientBillingHistoryAsync(int patientId, PaginationRequest pagination)
    {
        var billings = await _billingRepository.GetByPatientIdAsync(patientId);
        var pagedBillings = billings.Skip((pagination.PageNumber - 1) * pagination.PageSize).Take(pagination.PageSize).ToList();
        var billingDtos = _mapper.Map<List<BillingDto>>(pagedBillings);

        return PagedResponse<BillingDto>.Create(billingDtos, pagination.PageNumber, pagination.PageSize, billings.Count());
    }

    public async Task<PagedResponse<BillingDto>> GetAllBillingsAsync(PaginationRequest pagination, int? clinicId = null)
    {
        var billings = await _billingRepository.GetPagedAsync(pagination, clinicId);
        var billingDtos = _mapper.Map<List<BillingDto>>(billings);

        return PagedResponse<BillingDto>.Create(billingDtos, pagination.PageNumber, pagination.PageSize, billings.Count());
    }

    public async Task<PagedResponse<OutstandingBalanceReportDto>> GetOutstandingBalanceReportAsync(PaginationRequest pagination)
    {
        var outstandingBillings = await _billingRepository.GetOutstandingBalancesAsync();
        
        // Group by patient
        var report = outstandingBillings
            .GroupBy(b => b.PatientId)
            .Select(g => new OutstandingBalanceReportDto
            {
                PatientId = g.Key,
                PatientName = g.First().Patient.FullName,
                PatientPhone = g.First().Patient.PhoneNumber,
                TotalOutstanding = g.Sum(b => b.BalanceAmount),
                PendingInvoices = g.Count(b => b.BalanceAmount > 0)
            })
            .OrderByDescending(r => r.TotalOutstanding)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToList();

        return PagedResponse<OutstandingBalanceReportDto>.Create(report, pagination.PageNumber, pagination.PageSize, report.Count);
    }

    private async Task<string> GenerateInvoiceNumberAsync()
    {
        // Generate invoice number in format: INV-YYYYMMDD-XXXX
        var prefix = $"INV-{DateTime.Now:yyyyMMdd}-";
        var random = new Random();
        var suffix = random.Next(1000, 9999).ToString();
        
        // Ensure uniqueness
        var invoiceNumber = prefix + suffix;
        while (await _billingRepository.GetByInvoiceNumberAsync(invoiceNumber) != null)
        {
            suffix = random.Next(1000, 9999).ToString();
            invoiceNumber = prefix + suffix;
        }

        return invoiceNumber;
    }

    private async Task<BillingDto> MapToDto(Billing billing)
    {
        var dto = _mapper.Map<BillingDto>(billing);
        
        if (billing.Patient == null)
        {
            var patient = await _patientRepository.GetByIdAsync(billing.PatientId);
            dto.PatientName = patient?.FullName ?? "";
        }

        return dto;
    }
}
