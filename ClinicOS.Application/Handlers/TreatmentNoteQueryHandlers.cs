using ClinicOS.Application.DTOs;
using ClinicOS.Application.Interfaces;
using ClinicOS.Application.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ClinicOS.Application.Handlers;

/// <summary>
/// Handler for getting treatment note by ID
/// </summary>
public class GetTreatmentNoteByIdQueryHandler : IRequestHandler<GetTreatmentNoteByIdQuery, TreatmentNoteDto?>
{
    private readonly ITreatmentNoteRepository _repository;
    private readonly ILogger<GetTreatmentNoteByIdQueryHandler> _logger;

    public GetTreatmentNoteByIdQueryHandler(
        ITreatmentNoteRepository repository,
        ILogger<GetTreatmentNoteByIdQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<TreatmentNoteDto?> Handle(GetTreatmentNoteByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting treatment note by ID {Id}", request.Id);

        var treatmentNote = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (treatmentNote == null)
        {
            return null;
        }

        return MapToDto(treatmentNote);
    }

    private static TreatmentNoteDto MapToDto(ClinicOS.Domain.Entities.TreatmentNote note)
    {
        return new TreatmentNoteDto
        {
            Id = note.Id,
            ClinicId = note.ClinicId,
            PatientId = note.PatientId,
            AppointmentId = note.AppointmentId,
            ProcedureType = note.ProcedureType,
            ToothNumber = note.ToothNumber,
            Symptoms = note.Symptoms,
            Diagnosis = note.Diagnosis,
            TreatmentPerformed = note.TreatmentPerformed,
            AdditionalNotes = note.AdditionalNotes,
            AiGeneratedNote = note.AiGeneratedNote,
            FinalNote = note.FinalNote,
            GeneratedByUserId = note.GeneratedByUserId,
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt,
            GeneratedByUserName = note.GeneratedBy?.Username,
            PatientName = note.Patient?.FullName
        };
    }
}

/// <summary>
/// Handler for getting treatment notes by patient ID
/// </summary>
public class GetTreatmentNotesByPatientQueryHandler : IRequestHandler<GetTreatmentNotesByPatientQuery, List<TreatmentNoteDto>>
{
    private readonly ITreatmentNoteRepository _repository;
    private readonly ILogger<GetTreatmentNotesByPatientQueryHandler> _logger;

    public GetTreatmentNotesByPatientQueryHandler(
        ITreatmentNoteRepository repository,
        ILogger<GetTreatmentNotesByPatientQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<TreatmentNoteDto>> Handle(GetTreatmentNotesByPatientQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting treatment notes for patient {PatientId}", request.PatientId);

        var treatmentNotes = await _repository.GetByPatientIdAsync(
            request.PatientId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return treatmentNotes.Select(MapToDto).ToList();
    }

    private static TreatmentNoteDto MapToDto(ClinicOS.Domain.Entities.TreatmentNote note)
    {
        return new TreatmentNoteDto
        {
            Id = note.Id,
            ClinicId = note.ClinicId,
            PatientId = note.PatientId,
            AppointmentId = note.AppointmentId,
            ProcedureType = note.ProcedureType,
            ToothNumber = note.ToothNumber,
            Symptoms = note.Symptoms,
            Diagnosis = note.Diagnosis,
            TreatmentPerformed = note.TreatmentPerformed,
            AdditionalNotes = note.AdditionalNotes,
            AiGeneratedNote = note.AiGeneratedNote,
            FinalNote = note.FinalNote,
            GeneratedByUserId = note.GeneratedByUserId,
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt,
            GeneratedByUserName = note.GeneratedBy?.Username,
            PatientName = note.Patient?.FullName
        };
    }
}

/// <summary>
/// Handler for getting treatment notes by appointment ID
/// </summary>
public class GetTreatmentNotesByAppointmentQueryHandler : IRequestHandler<GetTreatmentNotesByAppointmentQuery, List<TreatmentNoteDto>>
{
    private readonly ITreatmentNoteRepository _repository;
    private readonly ILogger<GetTreatmentNotesByAppointmentQueryHandler> _logger;

    public GetTreatmentNotesByAppointmentQueryHandler(
        ITreatmentNoteRepository repository,
        ILogger<GetTreatmentNotesByAppointmentQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<TreatmentNoteDto>> Handle(GetTreatmentNotesByAppointmentQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting treatment notes for appointment {AppointmentId}", request.AppointmentId);

        var treatmentNotes = await _repository.GetByAppointmentIdAsync(request.AppointmentId, cancellationToken);

        return treatmentNotes.Select(MapToDto).ToList();
    }

    private static TreatmentNoteDto MapToDto(ClinicOS.Domain.Entities.TreatmentNote note)
    {
        return new TreatmentNoteDto
        {
            Id = note.Id,
            ClinicId = note.ClinicId,
            PatientId = note.PatientId,
            AppointmentId = note.AppointmentId,
            ProcedureType = note.ProcedureType,
            ToothNumber = note.ToothNumber,
            Symptoms = note.Symptoms,
            Diagnosis = note.Diagnosis,
            TreatmentPerformed = note.TreatmentPerformed,
            AdditionalNotes = note.AdditionalNotes,
            AiGeneratedNote = note.AiGeneratedNote,
            FinalNote = note.FinalNote,
            GeneratedByUserId = note.GeneratedByUserId,
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt,
            GeneratedByUserName = note.GeneratedBy?.Username,
            PatientName = note.Patient?.FullName
        };
    }
}
