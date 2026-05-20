using ClinicOS.Application.Common;
using ClinicOS.Application.DTOs;
using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Entities;
using FluentValidation;

namespace ClinicOS.Application.Services;

public class ClinicService : IClinicService
{
    private readonly IClinicRepository _clinicRepository;
    private readonly IValidator<CreateClinicDto> _createValidator;
    private readonly IValidator<UpdateClinicDto> _updateValidator;
    private readonly IUnitOfWork _unitOfWork;

    public ClinicService(
        IClinicRepository clinicRepository,
        IValidator<CreateClinicDto> createValidator,
        IValidator<UpdateClinicDto> updateValidator,
        IUnitOfWork unitOfWork)
    {
        _clinicRepository = clinicRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<ClinicDto>> CreateClinicAsync(CreateClinicDto dto, string createdBy)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ApiResponse<ClinicDto>.ErrorResponse("Validation failed",
                validation.Errors.Select(e => e.ErrorMessage).ToList());

        var code = dto.Code.Trim().ToLowerInvariant();
        if (await _clinicRepository.CodeExistsAsync(code))
            return ApiResponse<ClinicDto>.ErrorResponse("Clinic code already exists");

        var clinic = new Clinic
        {
            Code = code,
            Name = dto.Name,
            Address = dto.Address,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            City = dto.City,
            State = dto.State,
            PostalCode = dto.PostalCode,
            Country = dto.Country ?? "India",
            IsActive = true,
            CreatedBy = createdBy
        };

        await _clinicRepository.AddAsync(clinic);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<ClinicDto>.SuccessResponse(MapToDto(clinic), "Clinic created successfully");
    }

    public async Task<ApiResponse<ClinicDto>> UpdateClinicAsync(int id, UpdateClinicDto dto, string updatedBy)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ApiResponse<ClinicDto>.ErrorResponse("Validation failed",
                validation.Errors.Select(e => e.ErrorMessage).ToList());

        var clinic = await _clinicRepository.GetByIdAsync(id);
        if (clinic == null)
            return ApiResponse<ClinicDto>.ErrorResponse("Clinic not found");

        clinic.Name = dto.Name;
        clinic.Address = dto.Address;
        clinic.PhoneNumber = dto.PhoneNumber;
        clinic.Email = dto.Email;
        clinic.City = dto.City;
        clinic.State = dto.State;
        clinic.PostalCode = dto.PostalCode;
        clinic.Country = dto.Country;
        clinic.IsActive = dto.IsActive;
        clinic.UpdatedBy = updatedBy;

        _clinicRepository.Update(clinic);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<ClinicDto>.SuccessResponse(MapToDto(clinic), "Clinic updated successfully");
    }

    public async Task<ApiResponse<ClinicDto>> GetClinicByIdAsync(int id)
    {
        var clinic = await _clinicRepository.GetByIdAsync(id);
        if (clinic == null)
            return ApiResponse<ClinicDto>.ErrorResponse("Clinic not found");
        return ApiResponse<ClinicDto>.SuccessResponse(MapToDto(clinic));
    }

    public async Task<ApiResponse<ClinicDto>> GetClinicByCodeAsync(string code)
    {
        var clinic = await _clinicRepository.GetByCodeAsync(code.Trim().ToLowerInvariant());
        if (clinic == null)
            return ApiResponse<ClinicDto>.ErrorResponse("Clinic not found");
        return ApiResponse<ClinicDto>.SuccessResponse(MapToDto(clinic));
    }

    public async Task<PagedResponse<ClinicDto>> GetAllClinicsAsync(PaginationRequest pagination, bool? isActive = null)
    {
        var clinics = await _clinicRepository.GetPagedAsync(pagination, isActive);
        var total = await _clinicRepository.GetTotalCountAsync(isActive);
        var dtos = clinics.Select(MapToDto).ToList();

        return PagedResponse<ClinicDto>.Create(dtos, pagination.PageNumber, pagination.PageSize, total);
    }

    private static ClinicDto MapToDto(Clinic clinic) => new()
    {
        Id = clinic.Id,
        Code = clinic.Code,
        Name = clinic.Name,
        Address = clinic.Address,
        PhoneNumber = clinic.PhoneNumber,
        Email = clinic.Email,
        City = clinic.City,
        State = clinic.State,
        PostalCode = clinic.PostalCode,
        Country = clinic.Country,
        IsActive = clinic.IsActive,
        CreatedAt = clinic.CreatedAt
    };
}
