using ClinicOS.Application.Commands;
using ClinicOS.Application.DTOs;
using ClinicOS.Application.Interfaces;
using ClinicOS.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ClinicOS.API.Controllers;

/// <summary>
/// Controller for treatment notes management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TreatmentNotesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<TreatmentNotesController> _logger;

    public TreatmentNotesController(
        IMediator mediator,
        ITenantContext tenantContext,
        ILogger<TreatmentNotesController> logger)
    {
        _mediator = mediator;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    /// <summary>
    /// Generate AI treatment note
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<GenerateTreatmentNoteResponse>> GenerateTreatmentNote([FromBody] GenerateTreatmentNoteRequest request)
    {
        if (!_tenantContext.HasClinic)
        {
            return Unauthorized("Clinic context is required");
        }

        var command = new GenerateTreatmentNoteCommand
        {
            PatientId = request.PatientId,
            AppointmentId = request.AppointmentId,
            ProcedureType = request.ProcedureType,
            ToothNumber = request.ToothNumber,
            Symptoms = request.Symptoms,
            Diagnosis = request.Diagnosis,
            TreatmentPerformed = request.TreatmentPerformed,
            AdditionalNotes = request.AdditionalNotes
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Save treatment note
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TreatmentNoteDto>> SaveTreatmentNote([FromBody] CreateTreatmentNoteDto dto)
    {
        if (!_tenantContext.HasClinic)
        {
            return Unauthorized("Clinic context is required");
        }

        var command = new SaveTreatmentNoteCommand { TreatmentNote = dto };
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTreatmentNoteById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update treatment note
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<TreatmentNoteDto>> UpdateTreatmentNote(int id, [FromBody] UpdateTreatmentNoteDto dto)
    {
        if (!_tenantContext.HasClinic)
        {
            return Unauthorized("Clinic context is required");
        }

        var command = new UpdateTreatmentNoteCommand { Id = id, TreatmentNote = dto };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Delete treatment note
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTreatmentNote(int id)
    {
        if (!_tenantContext.HasClinic)
        {
            return Unauthorized("Clinic context is required");
        }

        var command = new DeleteTreatmentNoteCommand { Id = id };
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Get treatment note by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TreatmentNoteDto>> GetTreatmentNoteById(int id)
    {
        if (!_tenantContext.HasClinic)
        {
            return Unauthorized("Clinic context is required");
        }

        var query = new GetTreatmentNoteByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Get treatment notes by patient ID
    /// </summary>
    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<List<TreatmentNoteDto>>> GetTreatmentNotesByPatient(
        int patientId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        if (!_tenantContext.HasClinic)
        {
            return Unauthorized("Clinic context is required");
        }

        var query = new GetTreatmentNotesByPatientQuery
        {
            PatientId = patientId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get treatment notes by appointment ID
    /// </summary>
    [HttpGet("appointment/{appointmentId}")]
    public async Task<ActionResult<List<TreatmentNoteDto>>> GetTreatmentNotesByAppointment(int appointmentId)
    {
        if (!_tenantContext.HasClinic)
        {
            return Unauthorized("Clinic context is required");
        }

        var query = new GetTreatmentNotesByAppointmentQuery { AppointmentId = appointmentId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
