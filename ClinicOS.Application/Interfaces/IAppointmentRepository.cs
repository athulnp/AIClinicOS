using ClinicOS.Application.Common;
using ClinicOS.Domain.Entities;

namespace ClinicOS.Application.Interfaces;

/// <summary>
/// Appointment repository interface with specific operations
/// </summary>
public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId);
    Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId);
    Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Appointment>> GetDoctorScheduleAsync(int doctorId, DateTime date);
    Task<bool> HasOverlappingAppointmentAsync(int doctorId, DateTime date, TimeSpan startTime, TimeSpan endTime, int? excludeAppointmentId = null);
    Task<IEnumerable<Appointment>> GetPagedAsync(PaginationRequest pagination);
}
