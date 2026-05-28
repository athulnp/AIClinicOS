using ClinicOS.Application.DTOs;
using MediatR;

namespace ClinicOS.Application.Commands;

/// <summary>
/// Command to generate AI treatment note
/// </summary>
public class GenerateTreatmentNoteCommand : IRequest<GenerateTreatmentNoteResponse>
{
    public int PatientId { get; set; }
    public int AppointmentId { get; set; }
    public string ProcedureType { get; set; } = string.Empty;
    public string? ToothNumber { get; set; }
    public string? Symptoms { get; set; }
    public string? Diagnosis { get; set; }
    public string? TreatmentPerformed { get; set; }
    public string? AdditionalNotes { get; set; }
}

/// <summary>
/// Command to save treatment note
/// </summary>
public class SaveTreatmentNoteCommand : IRequest<TreatmentNoteDto>
{
    public CreateTreatmentNoteDto TreatmentNote { get; set; } = null!;
}

/// <summary>
/// Command to update treatment note
/// </summary>
public class UpdateTreatmentNoteCommand : IRequest<TreatmentNoteDto>
{
    public int Id { get; set; }
    public UpdateTreatmentNoteDto TreatmentNote { get; set; } = null!;
}

/// <summary>
/// Command to delete treatment note
/// </summary>
public class DeleteTreatmentNoteCommand : IRequest<bool>
{
    public int Id { get; set; }
}
