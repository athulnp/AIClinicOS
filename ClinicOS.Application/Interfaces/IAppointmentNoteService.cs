using ClinicOS.Application.Common;
using ClinicOS.Application.DTOs;
using ClinicOS.Domain.Entities;

namespace ClinicOS.Application.Interfaces;

public interface IAppointmentNoteRepository
{
    Task<List<AppointmentNote>> GetByAppointmentIdAsync(int appointmentId);
    Task<AppointmentNote?> GetByIdAsync(int id);
    Task<AppointmentNote> AddAsync(AppointmentNote note);
    void Update(AppointmentNote note);
    void Delete(AppointmentNote note);
    Task SaveAsync();
}

public interface IAppointmentNoteService
{
    Task<ApiResponse<List<AppointmentNoteDto>>> GetNotesByAppointmentIdAsync(int appointmentId);
    Task<ApiResponse<AppointmentNoteDto>> GetNoteByIdAsync(int id);
    Task<ApiResponse<AppointmentNoteDto>> CreateNoteAsync(int appointmentId, CreateAppointmentNoteDto dto, string createdBy);
    Task<ApiResponse<AppointmentNoteDto>> UpdateNoteAsync(int id, UpdateAppointmentNoteDto dto, string updatedBy);
    Task<ApiResponse> DeleteNoteAsync(int id);
}
