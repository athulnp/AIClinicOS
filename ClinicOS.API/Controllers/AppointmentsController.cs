using ClinicOS.Application.Common;
using ClinicOS.Application.DTOs;
using ClinicOS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [Authorize(Policy = PermissionCodes.AppointmentsRead)]
    [HttpGet]
    public async Task<ActionResult<PagedResponse<AppointmentDto>>> GetAllAppointments([FromQuery] PaginationRequest pagination)
    {
        var result = await _appointmentService.GetAllAppointmentsAsync(pagination);
        return Ok(result);
    }

    [Authorize(Policy = PermissionCodes.AppointmentsRead)]
    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentDto>> GetAppointment(int id)
    {
        var result = await _appointmentService.GetAppointmentByIdAsync(id);
        if (!result.Success)
            return NotFound(result);
        return Ok(result.Data);
    }

    [Authorize(Policy = PermissionCodes.AppointmentsRead)]
    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<PagedResponse<AppointmentDto>>> GetPatientAppointments(
        int patientId,
        [FromQuery] PaginationRequest pagination)
    {
        var result = await _appointmentService.GetPatientAppointmentsAsync(patientId, pagination);
        return Ok(result);
    }

    [Authorize(Policy = PermissionCodes.AppointmentsRead)]
    [HttpGet("doctor/{doctorId}/schedule")]
    public async Task<ActionResult<DoctorScheduleDto>> GetDoctorSchedule(
        int doctorId,
        [FromQuery] DateTime date)
    {
        var result = await _appointmentService.GetDoctorScheduleAsync(doctorId, date);
        if (!result.Success)
            return NotFound(result);
        return Ok(result.Data);
    }

    [Authorize(Policy = PermissionCodes.AppointmentsWrite)]
    [HttpPost]
    public async Task<ActionResult<AppointmentDto>> CreateAppointment([FromBody] CreateAppointmentDto dto)
    {
        var createdBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        var result = await _appointmentService.CreateAppointmentAsync(dto, createdBy);
        if (!result.Success)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetAppointment), new { id = result.Data!.Id }, result.Data);
    }

    [Authorize(Policy = PermissionCodes.AppointmentsWrite)]
    [HttpPut("{id}/reschedule")]
    public async Task<ActionResult<AppointmentDto>> RescheduleAppointment(
        int id,
        [FromBody] RescheduleAppointmentDto dto)
    {
        var updatedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        var result = await _appointmentService.RescheduleAppointmentAsync(id, dto, updatedBy);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result.Data);
    }

    [Authorize(Policy = PermissionCodes.AppointmentsWrite)]
    [HttpPut("{id}/status")]
    public async Task<ActionResult<AppointmentDto>> UpdateAppointmentStatus(
        int id,
        [FromBody] UpdateAppointmentStatusDto dto)
    {
        var updatedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        var result = await _appointmentService.UpdateAppointmentStatusAsync(id, dto, updatedBy);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result.Data);
    }

    [Authorize(Policy = PermissionCodes.AppointmentsWrite)]
    [HttpPut("{id}/cancel")]
    public async Task<ActionResult> CancelAppointment(int id)
    {
        var updatedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        var result = await _appointmentService.CancelAppointmentAsync(id, updatedBy);
        if (!result.Success)
            return NotFound(result);
        return Ok(result);
    }
}
