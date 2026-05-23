using ClinicOS.Application.Common;
using ClinicOS.Application.DTOs;
using ClinicOS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClinicsController : ControllerBase
{
    private readonly IClinicService _clinicService;

    public ClinicsController(IClinicService clinicService)
    {
        _clinicService = clinicService;
    }

    [Authorize(Policy = PermissionCodes.ClinicsWrite)]
    [HttpPost]
    public async Task<ActionResult<ClinicDto>> CreateClinic([FromBody] CreateClinicDto dto)
    {
        var createdBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        var result = await _clinicService.CreateClinicAsync(dto, createdBy);
        if (!result.Success)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetClinic), new { id = result.Data!.Id }, result.Data);
    }

    [Authorize(Policy = PermissionCodes.ClinicsRead)]
    [HttpGet]
    public async Task<ActionResult<PagedResponse<ClinicDto>>> GetClinics(
        [FromQuery] PaginationRequest pagination,
        [FromQuery] bool? isActive = null)
    {
        return Ok(await _clinicService.GetAllClinicsAsync(pagination, isActive));
    }

    [Authorize(Policy = PermissionCodes.ClinicsRead)]
    [HttpGet("{id}")]
    public async Task<ActionResult<ClinicDto>> GetClinic(int id)
    {
        var result = await _clinicService.GetClinicByIdAsync(id);
        if (!result.Success)
            return NotFound(result);
        return Ok(result.Data);
    }

    [AllowAnonymous]
    [HttpGet("code/{code}")]
    public async Task<ActionResult<ClinicDto>> GetClinicByCode(string code)
    {
        var result = await _clinicService.GetClinicByCodeAsync(code);
        if (!result.Success)
            return NotFound(result);
        return Ok(result.Data);
    }

    [Authorize(Policy = PermissionCodes.ClinicsWrite)]
    [HttpPut("{id}")]
    public async Task<ActionResult<ClinicDto>> UpdateClinic(int id, [FromBody] UpdateClinicDto dto)
    {
        var updatedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        var result = await _clinicService.UpdateClinicAsync(id, dto, updatedBy);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result.Data);
    }

    [Authorize(Policy = PermissionCodes.ClinicsWrite)]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteClinic(int id)
    {
        var deletedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        var result = await _clinicService.DeleteClinicAsync(id, deletedBy);
        if (!result.Success)
            return NotFound(result);
        return Ok(result);
    }
}
