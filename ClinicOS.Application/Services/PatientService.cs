using AutoMapper;
using ClinicOS.Application.Common;
using ClinicOS.Application.DTOs;
using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Entities;
using FluentValidation;

namespace ClinicOS.Application.Services;

/// <summary>
/// Patient service implementation
/// </summary>
public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IClinicRepository _clinicRepository;
    private readonly IValidator<CreatePatientDto> _createValidator;
    private readonly IValidator<UpdatePatientDto> _updateValidator;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public PatientService(
        IPatientRepository patientRepository,
        IClinicRepository clinicRepository,
        IValidator<CreatePatientDto> createValidator,
        IValidator<UpdatePatientDto> updateValidator,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _patientRepository = patientRepository;
        _clinicRepository = clinicRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PatientDto>> CreatePatientAsync(CreatePatientDto dto, string createdBy, int clinicId)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return ApiResponse<PatientDto>.ErrorResponse("Validation failed", validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        // Get clinic information
        var clinic = await _clinicRepository.GetByIdAsync(clinicId);
        if (clinic == null)
        {
            return ApiResponse<PatientDto>.ErrorResponse("Clinic not found");
        }

        // Generate PatientCode in format P-CLINICCODE-XXX (e.g., P-DEMO-001)
        var nextNumber = await _patientRepository.GetNextPatientNumberAsync(clinicId);
        var patientCode = $"P-{clinic.Code.ToUpper()}-{nextNumber:D3}";

        var patient = _mapper.Map<Patient>(dto);
        patient.PatientCode = patientCode;
        patient.ClinicId = clinicId;
        patient.CreatedBy = createdBy;

        await _patientRepository.AddAsync(patient);
        await _unitOfWork.SaveChangesAsync();

        var patientDto = _mapper.Map<PatientDto>(patient);
        return ApiResponse<PatientDto>.SuccessResponse(patientDto, "Patient created successfully");
    }

    public async Task<ApiResponse<PatientDto>> UpdatePatientAsync(int id, UpdatePatientDto dto, string updatedBy)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return ApiResponse<PatientDto>.ErrorResponse("Validation failed", validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient == null)
        {
            return ApiResponse<PatientDto>.ErrorResponse("Patient not found");
        }

        _mapper.Map(dto, patient);
        patient.UpdatedBy = updatedBy;

        _patientRepository.Update(patient);
        await _unitOfWork.SaveChangesAsync();

        var patientDto = _mapper.Map<PatientDto>(patient);
        return ApiResponse<PatientDto>.SuccessResponse(patientDto, "Patient updated successfully");
    }

    public async Task<ApiResponse> DeletePatientAsync(int id, string deletedBy)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient == null)
        {
            return ApiResponse.ErrorResponse("Patient not found");
        }

        await _patientRepository.SoftDeleteAsync(id, deletedBy);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse.SuccessResponse("Patient deleted successfully");
    }

    public async Task<ApiResponse<PatientDto>> GetPatientByIdAsync(int id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient == null)
        {
            return ApiResponse<PatientDto>.ErrorResponse("Patient not found");
        }

        var patientDto = _mapper.Map<PatientDto>(patient);
        return ApiResponse<PatientDto>.SuccessResponse(patientDto);
    }

    public async Task<ApiResponse<PatientDto>> GetPatientByCodeAsync(string patientCode)
    {
        var patient = await _patientRepository.GetByPatientCodeAsync(patientCode);
        if (patient == null)
        {
            return ApiResponse<PatientDto>.ErrorResponse("Patient not found");
        }

        var patientDto = _mapper.Map<PatientDto>(patient);
        return ApiResponse<PatientDto>.SuccessResponse(patientDto);
    }

    public async Task<PagedResponse<PatientDto>> SearchPatientsAsync(string? searchTerm, PaginationRequest pagination)
    {
        IEnumerable<Patient> patients;

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            patients = await _patientRepository.GetPagedAsync(pagination);
        }
        else
        {
            // Search by name or phone
            var byName = await _patientRepository.SearchByNameAsync(searchTerm);
            var byPhone = await _patientRepository.SearchByPhoneAsync(searchTerm);
            patients = byName.Union(byPhone).ToList();
        }

        var patientDtos = _mapper.Map<List<PatientDto>>(patients);
        var totalCount = await _patientRepository.GetTotalCountAsync();

        return PagedResponse<PatientDto>.Create(patientDtos, pagination.PageNumber, pagination.PageSize, totalCount);
    }

    public async Task<PagedResponse<PatientDto>> GetAllPatientsAsync(PaginationRequest pagination, int? clinicId = null)
    {
        var patients = await _patientRepository.GetPagedAsync(pagination, clinicId);
        var patientDtos = _mapper.Map<List<PatientDto>>(patients);
        var totalCount = await _patientRepository.GetTotalCountAsync();

        return PagedResponse<PatientDto>.Create(patientDtos, pagination.PageNumber, pagination.PageSize, totalCount);
    }
}
