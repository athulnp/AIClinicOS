using ClinicOS.Application.DTOs;
using MediatR;

namespace ClinicOS.Application.Queries;

/// <summary>
/// Query to get treatment note by ID
/// </summary>
public class GetTreatmentNoteByIdQuery : IRequest<TreatmentNoteDto?>
{
    public int Id { get; set; }
}

/// <summary>
/// Query to get treatment notes by patient ID
/// </summary>
public class GetTreatmentNotesByPatientQuery : IRequest<List<TreatmentNoteDto>>
{
    public int PatientId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// Query to get treatment notes by appointment ID
/// </summary>
public class GetTreatmentNotesByAppointmentQuery : IRequest<List<TreatmentNoteDto>>
{
    public int AppointmentId { get; set; }
}
