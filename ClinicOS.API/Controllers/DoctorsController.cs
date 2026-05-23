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
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<DoctorsController> _logger;

    public DoctorsController(IDoctorService doctorService, ILogger<DoctorsController> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    [Authorize(Policy = PermissionCodes.DoctorsRead)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DoctorResponseDto>>> GetAllDoctors()
    {
        try
        {
            var doctors = await _doctorService.GetAllDoctorsAsync();
            return Ok(doctors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all doctors");
            return StatusCode(500, "An error occurred while retrieving doctors");
        }
    }

    [Authorize(Policy = PermissionCodes.DoctorsRead)]
    [HttpGet("{id}")]
    public async Task<ActionResult<DoctorResponseDto>> GetDoctorById(int id)
    {
        try
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null)
                return NotFound(new { message = "Doctor not found" });
            return Ok(doctor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctor with ID {DoctorId}", id);
            return StatusCode(500, "An error occurred while retrieving doctor details");
        }
    }

    [Authorize(Policy = PermissionCodes.DoctorsRead)]
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<DoctorResponseDto>> GetDoctorByUserId(int userId)
    {
        try
        {
            var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
            if (doctor == null)
                return NotFound(new { message = "Doctor details not found for this user" });
            return Ok(doctor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctor for user {UserId}", userId);
            return StatusCode(500, "An error occurred while retrieving doctor details");
        }
    }

    [Authorize(Policy = PermissionCodes.DoctorsRead)]
    [HttpGet("license/{licenseNumber}")]
    public async Task<ActionResult<DoctorResponseDto>> GetDoctorByLicenseNumber(string licenseNumber)
    {
        try
        {
            var doctor = await _doctorService.GetDoctorByLicenseNumberAsync(licenseNumber);
            if (doctor == null)
                return NotFound(new { message = "Doctor not found with this license number" });
            return Ok(doctor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctor with license number {LicenseNumber}", licenseNumber);
            return StatusCode(500, "An error occurred while retrieving doctor details");
        }
    }

    [Authorize(Policy = PermissionCodes.DoctorsRead)]
    [HttpGet("specialization/{specialization}")]
    public async Task<ActionResult<IEnumerable<DoctorResponseDto>>> GetDoctorsBySpecialization(string specialization)
    {
        try
        {
            var doctors = await _doctorService.GetDoctorsBySpecializationAsync(specialization);
            return Ok(doctors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctors with specialization {Specialization}", specialization);
            return StatusCode(500, "An error occurred while retrieving doctors");
        }
    }

    [Authorize(Policy = PermissionCodes.DoctorsRead)]
    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<DoctorResponseDto>>> GetAvailableDoctors()
    {
        try
        {
            var doctors = await _doctorService.GetAvailableDoctorsAsync();
            return Ok(doctors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving available doctors");
            return StatusCode(500, "An error occurred while retrieving available doctors");
        }
    }

    [Authorize(Policy = PermissionCodes.DoctorsManage)]
    [HttpPost]
    public async Task<ActionResult<DoctorResponseDto>> CreateDoctor([FromBody] CreateDoctorWithUserDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int? clinicId;
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
                clinicId = int.TryParse(clinicIdClaim, out var cid) ? cid : null;
            }

            var doctor = await _doctorService.CreateDoctorWithUserAsync(dto, clinicId);
            return CreatedAtAction(nameof(GetDoctorById), new { id = doctor.Id }, doctor);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation when creating doctor");
            return BadRequest(new { message = ex.Message });
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error when creating doctor");
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating doctor");
            return StatusCode(500, "An error occurred while creating doctor details");
        }
    }

    [Authorize(Policy = PermissionCodes.DoctorsWrite)]
    [HttpPut("{id}")]
    public async Task<ActionResult<DoctorResponseDto>> UpdateDoctor(int id, [FromBody] UpdateDoctorDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var doctor = await _doctorService.UpdateDoctorAsync(id, dto);
            return Ok(doctor);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation when updating doctor {DoctorId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error when updating doctor {DoctorId}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating doctor {DoctorId}", id);
            return StatusCode(500, "An error occurred while updating doctor details");
        }
    }

    [Authorize(Policy = PermissionCodes.DoctorsManage)]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteDoctor(int id)
    {
        try
        {
            var result = await _doctorService.DeleteDoctorAsync(id);
            if (!result)
                return NotFound(new { message = "Doctor not found" });
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation when deleting doctor {DoctorId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting doctor {DoctorId}", id);
            return StatusCode(500, "An error occurred while deleting doctor details");
        }
    }
}
