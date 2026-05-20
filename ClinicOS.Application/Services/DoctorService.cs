using AutoMapper;
using ClinicOS.Application.Common;
using ClinicOS.Application.DTOs;
using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Entities;
using ClinicOS.Domain.Enums;
using FluentValidation;

namespace ClinicOS.Application.Services;

/// <summary>
/// Doctor service implementation
/// </summary>
public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IUserRepository _userRepository;
    private readonly IValidator<CreateDoctorDto> _createValidator;
    private readonly IValidator<UpdateDoctorDto> _updateValidator;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public DoctorService(
        IDoctorRepository doctorRepository,
        IUserRepository userRepository,
        IValidator<CreateDoctorDto> createValidator,
        IValidator<UpdateDoctorDto> updateValidator,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _doctorRepository = doctorRepository;
        _userRepository = userRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<DoctorResponseDto?> GetDoctorByIdAsync(int id)
    {
        var doctor = await _doctorRepository.GetByIdAsync(id);
        return doctor == null ? null : _mapper.Map<DoctorResponseDto>(doctor);
    }

    public async Task<DoctorResponseDto?> GetDoctorByUserIdAsync(int userId)
    {
        var doctor = await _doctorRepository.GetByUserIdAsync(userId);
        return doctor == null ? null : _mapper.Map<DoctorResponseDto>(doctor);
    }

    public async Task<DoctorResponseDto?> GetDoctorByLicenseNumberAsync(string licenseNumber)
    {
        var doctor = await _doctorRepository.GetByLicenseNumberAsync(licenseNumber);
        return doctor == null ? null : _mapper.Map<DoctorResponseDto>(doctor);
    }

    public async Task<IEnumerable<DoctorResponseDto>> GetAllDoctorsAsync()
    {
        var doctors = await _doctorRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<DoctorResponseDto>>(doctors);
    }

    public async Task<IEnumerable<DoctorResponseDto>> GetDoctorsBySpecializationAsync(string specialization)
    {
        var doctors = await _doctorRepository.GetBySpecializationAsync(specialization);
        return _mapper.Map<IEnumerable<DoctorResponseDto>>(doctors);
    }

    public async Task<IEnumerable<DoctorResponseDto>> GetAvailableDoctorsAsync()
    {
        var doctors = await _doctorRepository.GetAvailableDoctorsAsync();
        return _mapper.Map<IEnumerable<DoctorResponseDto>>(doctors);
    }

    public async Task<DoctorResponseDto> CreateDoctorAsync(CreateDoctorDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(string.Join(", ", errors));
        }

        // Verify user exists and is a doctor
        var user = await _userRepository.GetByIdAsync(dto.UserId);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {dto.UserId} not found");
        }

        if (user.Role != UserRole.Doctor)
        {
            throw new InvalidOperationException("User must have Doctor role");
        }

        // Check if doctor already has details
        var existingDoctor = await _doctorRepository.GetByUserIdAsync(dto.UserId);
        if (existingDoctor != null)
        {
            throw new InvalidOperationException("Doctor details already exist for this user");
        }

        // Check if license number already exists
        if (await _doctorRepository.LicenseNumberExistsAsync(dto.LicenseNumber))
        {
            throw new InvalidOperationException("License number already exists");
        }

        var doctor = _mapper.Map<Doctor>(dto);
        doctor.ClinicId = user.ClinicId!.Value;
        await _doctorRepository.CreateAsync(doctor);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<DoctorResponseDto>(doctor);
    }

    public async Task<DoctorResponseDto> UpdateDoctorAsync(int id, UpdateDoctorDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(string.Join(", ", errors));
        }

        var doctor = await _doctorRepository.GetByIdAsync(id);
        if (doctor == null)
        {
            throw new InvalidOperationException($"Doctor with ID {id} not found");
        }

        // Check if new license number already exists (if being updated)
        if (!string.IsNullOrEmpty(dto.LicenseNumber) && dto.LicenseNumber != doctor.LicenseNumber)
        {
            if (await _doctorRepository.LicenseNumberExistsAsync(dto.LicenseNumber, id))
            {
                throw new InvalidOperationException("License number already exists");
            }
        }

        _mapper.Map(dto, doctor);
        await _doctorRepository.UpdateAsync(doctor);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<DoctorResponseDto>(doctor);
    }

    public async Task<bool> DeleteDoctorAsync(int id)
    {
        if (!await _doctorRepository.ExistsAsync(id))
        {
            throw new InvalidOperationException($"Doctor with ID {id} not found");
        }

        var result = await _doctorRepository.DeleteAsync(id);
        if (result)
        {
            await _unitOfWork.SaveChangesAsync();
        }

        return result;
    }
}
