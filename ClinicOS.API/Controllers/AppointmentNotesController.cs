using ClinicOS.Application.DTOs;
using ClinicOS.Application.Interfaces;
using ClinicOS.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicOS.API.Controllers;

[ApiController]
[Route("api/appointments/{appointmentId}/notes")]
[Authorize]
public class AppointmentNotesController : ControllerBase
{
    private readonly IAppointmentNoteService _noteService;

    public AppointmentNotesController(IAppointmentNoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<AppointmentNoteDto>>>> GetNotes(int appointmentId)
    {
        var result = await _noteService.GetNotesByAppointmentIdAsync(appointmentId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<AppointmentNoteDto>>> GetNote(int appointmentId, int id)
    {
        var result = await _noteService.GetNoteByIdAsync(id);
        if (!result.Success)
            return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<AppointmentNoteDto>>> CreateNote(int appointmentId, [FromBody] CreateAppointmentNoteDto dto)
    {
        var createdBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        var result = await _noteService.CreateNoteAsync(appointmentId, dto, createdBy);
        if (!result.Success)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetNote), new { appointmentId, id = result.Data!.Id }, result.Data);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<AppointmentNoteDto>>> UpdateNote(int appointmentId, int id, [FromBody] UpdateAppointmentNoteDto dto)
    {
        var updatedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        var result = await _noteService.UpdateNoteAsync(id, dto, updatedBy);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteNote(int appointmentId, int id)
    {
        var result = await _noteService.DeleteNoteAsync(id);
        if (!result.Success)
            return NotFound(result);
        return Ok(result);
    }
}
