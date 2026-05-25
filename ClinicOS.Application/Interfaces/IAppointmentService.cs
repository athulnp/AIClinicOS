using ClinicOS.Application.Common;
using ClinicOS.Application.DTOs;

namespace ClinicOS.Application.Interfaces;

/// <summary>
/// Appointment service interface
/// </summary>
public interface IAppointmentService
{
    Task<ApiResponse<AppointmentDto>> CreateAppointmentAsync(CreateAppointmentDto dto, string createdBy, int userId, int? clinicId = null);
    Task<ApiResponse<AppointmentDto>> RescheduleAppointmentAsync(int id, RescheduleAppointmentDto dto, string updatedBy, int userId);
    Task<ApiResponse<AppointmentDto>> UpdateAppointmentStatusAsync(int id, UpdateAppointmentStatusDto dto, string updatedBy, int userId);
    Task<ApiResponse<AppointmentDto>> UpdateAppointmentAsync(int id, UpdateAppointmentDto dto, string updatedBy, int userId);
    Task<ApiResponse> CancelAppointmentAsync(int id, string updatedBy, int userId);
    Task<ApiResponse<AppointmentDto>> GetAppointmentByIdAsync(int id);
    Task<PagedResponse<AppointmentDto>> GetPatientAppointmentsAsync(int patientId, PaginationRequest pagination);
    Task<ApiResponse<DoctorScheduleDto>> GetDoctorScheduleAsync(int doctorId, DateTime date);
    Task<PagedResponse<AppointmentDto>> GetAllAppointmentsAsync(PaginationRequest pagination, int? clinicId = null);
}
