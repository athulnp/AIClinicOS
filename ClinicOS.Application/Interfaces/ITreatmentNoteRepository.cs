using ClinicOS.Application.DTOs;
using ClinicOS.Domain.Entities;

namespace ClinicOS.Application.Interfaces;

/// <summary>
/// Repository interface for TreatmentNote entity
/// </summary>
public interface ITreatmentNoteRepository
{
    Task<TreatmentNote?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<TreatmentNote>> GetByPatientIdAsync(int patientId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<List<TreatmentNote>> GetByAppointmentIdAsync(int appointmentId, CancellationToken cancellationToken = default);
    Task<TreatmentNote> AddAsync(TreatmentNote treatmentNote, CancellationToken cancellationToken = default);
    Task<TreatmentNote> UpdateAsync(TreatmentNote treatmentNote, CancellationToken cancellationToken = default);
    Task DeleteAsync(TreatmentNote treatmentNote, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}
