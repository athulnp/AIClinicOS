using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Entities;
using ClinicOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicOS.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for TreatmentNote entity
/// </summary>
public class TreatmentNoteRepository : ITreatmentNoteRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<TreatmentNoteRepository> _logger;

    public TreatmentNoteRepository(AppDbContext context, ILogger<TreatmentNoteRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TreatmentNote?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.TreatmentNotes
            .Include(t => t.Patient)
            .Include(t => t.Appointment)
            .Include(t => t.GeneratedBy)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<List<TreatmentNote>> GetByPatientIdAsync(int patientId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.TreatmentNotes
            .Include(t => t.Patient)
            .Include(t => t.Appointment)
            .Include(t => t.GeneratedBy)
            .Where(t => t.PatientId == patientId)
            .OrderByDescending(t => t.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TreatmentNote>> GetByAppointmentIdAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Querying treatment notes for appointment {AppointmentId}", appointmentId);
        var notes = await _context.TreatmentNotes
            .Include(t => t.Patient)
            .Include(t => t.Appointment)
            .Include(t => t.GeneratedBy)
            .Where(t => t.AppointmentId == appointmentId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
        
        _logger.LogInformation("Found {Count} treatment notes for appointment {AppointmentId}", notes.Count, appointmentId);
        foreach (var note in notes)
        {
            _logger.LogInformation("Note ID: {Id}, ClinicId: {ClinicId}, AppointmentId: {AppointmentId}", note.Id, note.ClinicId, note.AppointmentId);
        }
        
        return notes;
    }

    public async Task<TreatmentNote> AddAsync(TreatmentNote treatmentNote, CancellationToken cancellationToken = default)
    {
        _context.TreatmentNotes.Add(treatmentNote);
        await _context.SaveChangesAsync(cancellationToken);
        return treatmentNote;
    }

    public async Task<TreatmentNote> UpdateAsync(TreatmentNote treatmentNote, CancellationToken cancellationToken = default)
    {
        _context.TreatmentNotes.Update(treatmentNote);
        await _context.SaveChangesAsync(cancellationToken);
        return treatmentNote;
    }

    public async Task DeleteAsync(TreatmentNote treatmentNote, CancellationToken cancellationToken = default)
    {
        _context.TreatmentNotes.Remove(treatmentNote);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.TreatmentNotes.AnyAsync(t => t.Id == id, cancellationToken);
    }
}
