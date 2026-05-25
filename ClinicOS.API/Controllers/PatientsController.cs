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
        // Note: TenantMiddleware sets clinic_id from JWT claims, and global query filter handles filtering
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
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        int clinicId;
        var isSuperAdmin = User.IsInRole(RoleNames.SuperAdmin);
        
        if (isSuperAdmin)
        {
            // Super admins must provide clinicId in the request body
            if (!dto.ClinicId.HasValue)
            {
                return BadRequest("ClinicId is required for super admins");
            }
            clinicId = dto.ClinicId.Value;
        }
        else
        {
            // Regular staff use clinic_id from claims or header
            var clinicIdClaim = User.FindFirst("clinic_id")?.Value;
            if (string.IsNullOrEmpty(clinicIdClaim))
            {
                return BadRequest("Clinic context not found");
            }
            
            if (!int.TryParse(clinicIdClaim, out clinicId))
            {
                return BadRequest("Invalid clinic context");
            }
        }

        var result = await _patientService.CreatePatientAsync(dto, createdBy, userId, clinicId);
        if (!result.Success)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetPatient), new { id = result.Data!.Id }, result.Data);
    }

    [Authorize(Policy = PermissionCodes.PatientsWrite)]
    [HttpPut("{id}")]
    public async Task<ActionResult<PatientDto>> UpdatePatient(int id, [FromBody] UpdatePatientDto dto)
    {
        var updatedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _patientService.UpdatePatientAsync(id, dto, updatedBy, userId);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result.Data);
    }

    [Authorize(Policy = PermissionCodes.PatientsWrite)]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePatient(int id)
    {
        var deletedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _patientService.DeletePatientAsync(id, deletedBy, userId);
        if (!result.Success)
            return NotFound(result);
        return Ok(result);
    }
}
