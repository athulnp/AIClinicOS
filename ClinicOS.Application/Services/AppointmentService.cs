using AutoMapper;
using ClinicOS.Application.Common;
using ClinicOS.Application.DTOs;
using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Entities;
using ClinicOS.Domain.Enums;
using FluentValidation;

namespace ClinicOS.Application.Services;

/// <summary>
/// Appointment service implementation
/// </summary>
public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IValidator<CreateAppointmentDto> _createValidator;
    private readonly IValidator<RescheduleAppointmentDto> _rescheduleValidator;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        IPatientRepository patientRepository,
        IValidator<CreateAppointmentDto> createValidator,
        IValidator<RescheduleAppointmentDto> rescheduleValidator,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _appointmentRepository = appointmentRepository;
        _patientRepository = patientRepository;
        _createValidator = createValidator;
        _rescheduleValidator = rescheduleValidator;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<AppointmentDto>> CreateAppointmentAsync(CreateAppointmentDto dto, string createdBy, int? clinicId = null)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return ApiResponse<AppointmentDto>.ErrorResponse("Validation failed", validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        // Check if patient exists
        var patient = await _patientRepository.GetByIdAsync(dto.PatientId);
        if (patient == null)
        {
            return ApiResponse<AppointmentDto>.ErrorResponse("Patient not found");
        }

        // Check for overlapping appointments
        var hasOverlap = await _appointmentRepository.HasOverlappingAppointmentAsync(
            dto.DoctorId, dto.AppointmentDate, dto.StartTime, dto.EndTime);

        if (hasOverlap)
        {
            return ApiResponse<AppointmentDto>.ErrorResponse("Doctor has an overlapping appointment at this time");
        }

        var appointment = _mapper.Map<Appointment>(dto);
        // Use clinicId from parameter if provided (for super admins), otherwise use patient's clinic
        appointment.ClinicId = clinicId ?? patient.ClinicId;
        appointment.Status = AppointmentStatus.Scheduled;
        appointment.CreatedBy = createdBy;

        await _appointmentRepository.AddAsync(appointment);
        await _unitOfWork.SaveChangesAsync();

        var appointmentDto = await MapToDto(appointment);
        return ApiResponse<AppointmentDto>.SuccessResponse(appointmentDto, "Appointment created successfully");
    }

    public async Task<ApiResponse<AppointmentDto>> RescheduleAppointmentAsync(int id, RescheduleAppointmentDto dto, string updatedBy)
    {
        var validationResult = await _rescheduleValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return ApiResponse<AppointmentDto>.ErrorResponse("Validation failed", validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment == null)
        {
            return ApiResponse<AppointmentDto>.ErrorResponse("Appointment not found");
        }

        // Check for overlapping appointments (excluding current appointment)
        var hasOverlap = await _appointmentRepository.HasOverlappingAppointmentAsync(
            appointment.DoctorId, dto.NewAppointmentDate, dto.NewStartTime, dto.NewEndTime, id);

        if (hasOverlap)
        {
            return ApiResponse<AppointmentDto>.ErrorResponse("Doctor has an overlapping appointment at this time");
        }

        appointment.AppointmentDate = dto.NewAppointmentDate;
        appointment.StartTime = dto.NewStartTime;
        appointment.EndTime = dto.NewEndTime;
        appointment.UpdatedBy = updatedBy;

        _appointmentRepository.Update(appointment);
        await _unitOfWork.SaveChangesAsync();

        var appointmentDto = await MapToDto(appointment);
        return ApiResponse<AppointmentDto>.SuccessResponse(appointmentDto, "Appointment rescheduled successfully");
    }

    public async Task<ApiResponse<AppointmentDto>> UpdateAppointmentStatusAsync(int id, UpdateAppointmentStatusDto dto, string updatedBy)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment == null)
        {
            return ApiResponse<AppointmentDto>.ErrorResponse("Appointment not found");
        }

        appointment.Status = dto.Status;
        appointment.Notes = dto.Notes ?? appointment.Notes;
        appointment.UpdatedBy = updatedBy;

        _appointmentRepository.Update(appointment);
        await _unitOfWork.SaveChangesAsync();

        var appointmentDto = await MapToDto(appointment);
        return ApiResponse<AppointmentDto>.SuccessResponse(appointmentDto, "Appointment status updated successfully");
    }

    public async Task<ApiResponse> CancelAppointmentAsync(int id, string updatedBy)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment == null)
        {
            return ApiResponse.ErrorResponse("Appointment not found");
        }

        appointment.Status = AppointmentStatus.Cancelled;
        appointment.UpdatedBy = updatedBy;

        _appointmentRepository.Update(appointment);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse.SuccessResponse("Appointment cancelled successfully");
    }

    public async Task<ApiResponse<AppointmentDto>> GetAppointmentByIdAsync(int id)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment == null)
        {
            return ApiResponse<AppointmentDto>.ErrorResponse("Appointment not found");
        }

        var appointmentDto = await MapToDto(appointment);
        return ApiResponse<AppointmentDto>.SuccessResponse(appointmentDto);
    }

    public async Task<PagedResponse<AppointmentDto>> GetPatientAppointmentsAsync(int patientId, PaginationRequest pagination)
    {
        var appointments = await _appointmentRepository.GetByPatientIdAsync(patientId);
        var pagedAppointments = appointments.Skip((pagination.PageNumber - 1) * pagination.PageSize).Take(pagination.PageSize).ToList();
        var appointmentDtos = await Task.WhenAll(pagedAppointments.Select(MapToDto));

        return PagedResponse<AppointmentDto>.Create(appointmentDtos.ToList(), pagination.PageNumber, pagination.PageSize, appointments.Count());
    }

    public async Task<ApiResponse<DoctorScheduleDto>> GetDoctorScheduleAsync(int doctorId, DateTime date)
    {
        var appointments = await _appointmentRepository.GetDoctorScheduleAsync(doctorId, date);
        var appointmentDtos = await Task.WhenAll(appointments.Select(MapToDto));

        var schedule = new DoctorScheduleDto
        {
            DoctorId = doctorId,
            DoctorName = appointmentDtos.FirstOrDefault()?.DoctorName ?? "",
            Date = date,
            Appointments = appointmentDtos.ToList()
        };

        return ApiResponse<DoctorScheduleDto>.SuccessResponse(schedule);
    }

    public async Task<PagedResponse<AppointmentDto>> GetAllAppointmentsAsync(PaginationRequest pagination, int? clinicId = null)
    {
        var appointments = await _appointmentRepository.GetPagedAsync(pagination, clinicId);
        var appointmentDtos = await Task.WhenAll(appointments.Select(MapToDto));

        return PagedResponse<AppointmentDto>.Create(appointmentDtos.ToList(), pagination.PageNumber, pagination.PageSize, appointments.Count());
    }

    private async Task<AppointmentDto> MapToDto(Appointment appointment)
    {
        var dto = _mapper.Map<AppointmentDto>(appointment);
        
        // Load related entities if not already loaded
        if (appointment.Patient == null)
        {
            var patient = await _patientRepository.GetByIdAsync(appointment.PatientId);
            dto.PatientName = patient?.FullName ?? "";
        }

        return dto;
    }
}
