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
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [Authorize(Policy = PermissionCodes.PatientsRead)]
    [HttpGet]
    public async Task<ActionResult<PagedResponse<PatientDto>>> GetAllPatients([FromQuery] PaginationRequest pagination)
    {
        var result = await _patientService.GetAllPatientsAsync(pagination);
        return Ok(result);
    }

    [Authorize(Policy = PermissionCodes.PatientsRead)]
    [HttpGet("search")]
    public async Task<ActionResult<PagedResponse<PatientDto>>> SearchPatients(
        [FromQuery] string? searchTerm,
        [FromQuery] PaginationRequest pagination)
    {
        var result = await _patientService.SearchPatientsAsync(searchTerm, pagination);
        return Ok(result);
    }

    [Authorize(Policy = PermissionCodes.PatientsRead)]
    [HttpGet("{id}")]
    public async Task<ActionResult<PatientDto>> GetPatient(int id)
    {
        var result = await _patientService.GetPatientByIdAsync(id);
        if (!result.Success)
            return NotFound(result);
        return Ok(result.Data);
    }

    [Authorize(Policy = PermissionCodes.PatientsRead)]
    [HttpGet("code/{patientCode}")]
    public async Task<ActionResult<PatientDto>> GetPatientByCode(string patientCode)
    {
        var result = await _patientService.GetPatientByCodeAsync(patientCode);
        if (!result.Success)
            return NotFound(result);
        return Ok(result.Data);
    }

    [Authorize(Policy = PermissionCodes.PatientsWrite)]
    [HttpPost]
    public async Task<ActionResult<PatientDto>> CreatePatient([FromBody] CreatePatientDto dto)
    {
        var createdBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        var result = await _patientService.CreatePatientAsync(dto, createdBy);
        if (!result.Success)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetPatient), new { id = result.Data!.Id }, result.Data);
    }

    [Authorize(Policy = PermissionCodes.PatientsWrite)]
    [HttpPut("{id}")]
    public async Task<ActionResult<PatientDto>> UpdatePatient(int id, [FromBody] UpdatePatientDto dto)
    {
        var updatedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        var result = await _patientService.UpdatePatientAsync(id, dto, updatedBy);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result.Data);
    }

    [Authorize(Policy = PermissionCodes.PatientsWrite)]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePatient(int id)
    {
        var deletedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        var result = await _patientService.DeletePatientAsync(id, deletedBy);
        if (!result.Success)
            return NotFound(result);
        return Ok(result);
    }
}
