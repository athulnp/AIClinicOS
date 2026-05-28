using ClinicOS.Application.Commands;
using ClinicOS.Application.DTOs;
using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ClinicOS.Application.Handlers;

/// <summary>
/// Handler for generating AI treatment note
/// </summary>
public class GenerateTreatmentNoteCommandHandler : IRequestHandler<GenerateTreatmentNoteCommand, GenerateTreatmentNoteResponse>
{
    private readonly IAIClinicalNotesService _aiService;
    private readonly IAiUsageLogRepository _aiUsageLogRepository;
    private readonly ILogger<GenerateTreatmentNoteCommandHandler> _logger;

    public GenerateTreatmentNoteCommandHandler(
        IAIClinicalNotesService aiService,
        IAiUsageLogRepository aiUsageLogRepository,
        ILogger<GenerateTreatmentNoteCommandHandler> logger)
    {
        _aiService = aiService;
        _aiUsageLogRepository = aiUsageLogRepository;
        _logger = logger;
    }

    public async Task<GenerateTreatmentNoteResponse> Handle(GenerateTreatmentNoteCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating AI treatment note for appointment {AppointmentId}", request.AppointmentId);

        var generatedNote = await _aiService.GenerateTreatmentNoteAsync(
            request.ProcedureType,
            request.ToothNumber,
            request.Symptoms,
            request.Diagnosis,
            request.TreatmentPerformed,
            request.AdditionalNotes,
            cancellationToken);

        // Log AI usage (simplified - in production you'd track actual tokens/cost from the AI provider response)
        var usageLog = new AiUsageLog
        {
            FeatureName = "TreatmentNotesGenerator",
            Provider = _aiService.GetProviderName(),
            Model = "gemini-1.5-flash",
            PromptTokens = null, // Would be populated from actual API response
            CompletionTokens = null,
            TotalTokens = null,
            EstimatedCost = null
        };

        await _aiUsageLogRepository.AddAsync(usageLog, cancellationToken);

        return new GenerateTreatmentNoteResponse { GeneratedNote = generatedNote };
    }
}

/// <summary>
/// Handler for saving treatment note
/// </summary>
public class SaveTreatmentNoteCommandHandler : IRequestHandler<SaveTreatmentNoteCommand, TreatmentNoteDto>
{
    private readonly ITreatmentNoteRepository _repository;
    private readonly ILogger<SaveTreatmentNoteCommandHandler> _logger;
    private readonly ITenantContext _tenantContext;

    public SaveTreatmentNoteCommandHandler(
        ITreatmentNoteRepository repository,
        ILogger<SaveTreatmentNoteCommandHandler> logger,
        ITenantContext tenantContext)
    {
        _repository = repository;
        _logger = logger;
        _tenantContext = tenantContext;
    }

    public async Task<TreatmentNoteDto> Handle(SaveTreatmentNoteCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Saving treatment note for patient {PatientId}", request.TreatmentNote.PatientId);

        var treatmentNote = new TreatmentNote
        {
            ClinicId = _tenantContext.ClinicId ?? 1, // Fallback to clinic 1 if no context
            PatientId = request.TreatmentNote.PatientId,
            AppointmentId = request.TreatmentNote.AppointmentId,
            ProcedureType = request.TreatmentNote.ProcedureType,
            ToothNumber = request.TreatmentNote.ToothNumber,
            Symptoms = request.TreatmentNote.Symptoms,
            Diagnosis = request.TreatmentNote.Diagnosis,
            TreatmentPerformed = request.TreatmentNote.TreatmentPerformed,
            AdditionalNotes = request.TreatmentNote.AdditionalNotes,
            AiGeneratedNote = request.TreatmentNote.AiGeneratedNote,
            FinalNote = request.TreatmentNote.FinalNote,
            GeneratedByUserId = 1 // TODO: Get from current user context
        };

        var savedNote = await _repository.AddAsync(treatmentNote, cancellationToken);

        return MapToDto(savedNote);
    }

    private static TreatmentNoteDto MapToDto(TreatmentNote note)
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
/// Handler for updating treatment note
/// </summary>
public class UpdateTreatmentNoteCommandHandler : IRequestHandler<UpdateTreatmentNoteCommand, TreatmentNoteDto>
{
    private readonly ITreatmentNoteRepository _repository;
    private readonly ILogger<UpdateTreatmentNoteCommandHandler> _logger;

    public UpdateTreatmentNoteCommandHandler(
        ITreatmentNoteRepository repository,
        ILogger<UpdateTreatmentNoteCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<TreatmentNoteDto> Handle(UpdateTreatmentNoteCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating treatment note {Id}", request.Id);

        var treatmentNote = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (treatmentNote == null)
        {
            throw new InvalidOperationException($"Treatment note with ID {request.Id} not found");
        }

        if (request.TreatmentNote.ProcedureType != null)
            treatmentNote.ProcedureType = request.TreatmentNote.ProcedureType;
        if (request.TreatmentNote.ToothNumber != null)
            treatmentNote.ToothNumber = request.TreatmentNote.ToothNumber;
        if (request.TreatmentNote.Symptoms != null)
            treatmentNote.Symptoms = request.TreatmentNote.Symptoms;
        if (request.TreatmentNote.Diagnosis != null)
            treatmentNote.Diagnosis = request.TreatmentNote.Diagnosis;
        if (request.TreatmentNote.TreatmentPerformed != null)
            treatmentNote.TreatmentPerformed = request.TreatmentNote.TreatmentPerformed;
        if (request.TreatmentNote.AdditionalNotes != null)
            treatmentNote.AdditionalNotes = request.TreatmentNote.AdditionalNotes;
        if (request.TreatmentNote.FinalNote != null)
            treatmentNote.FinalNote = request.TreatmentNote.FinalNote;

        var updatedNote = await _repository.UpdateAsync(treatmentNote, cancellationToken);

        return MapToDto(updatedNote);
    }

    private static TreatmentNoteDto MapToDto(TreatmentNote note)
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
/// Handler for deleting treatment note
/// </summary>
public class DeleteTreatmentNoteCommandHandler : IRequestHandler<DeleteTreatmentNoteCommand, bool>
{
    private readonly ITreatmentNoteRepository _repository;
    private readonly ILogger<DeleteTreatmentNoteCommandHandler> _logger;

    public DeleteTreatmentNoteCommandHandler(
        ITreatmentNoteRepository repository,
        ILogger<DeleteTreatmentNoteCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteTreatmentNoteCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting treatment note {Id}", request.Id);

        var treatmentNote = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (treatmentNote == null)
        {
            return false;
        }

        await _repository.DeleteAsync(treatmentNote, cancellationToken);
        return true;
    }
}
