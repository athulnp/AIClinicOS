using ClinicOS.Application.DTOs;
using ClinicOS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicOS.API.Controllers;

/// <summary>
/// Doctor management controller
/// </summary>
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

    /// <summary>
    /// Get all doctors
    /// </summary>
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

    /// <summary>
    /// Get doctor by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<DoctorResponseDto>> GetDoctorById(int id)
    {
        try
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null)
            {
                return NotFound(new { message = "Doctor not found" });
            }
            return Ok(doctor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctor with ID {DoctorId}", id);
            return StatusCode(500, "An error occurred while retrieving doctor details");
        }
    }

    /// <summary>
    /// Get doctor by user ID
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<DoctorResponseDto>> GetDoctorByUserId(int userId)
    {
        try
        {
            var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
            if (doctor == null)
            {
                return NotFound(new { message = "Doctor details not found for this user" });
            }
            return Ok(doctor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctor for user {UserId}", userId);
            return StatusCode(500, "An error occurred while retrieving doctor details");
        }
    }

    /// <summary>
    /// Get doctor by license number
    /// </summary>
    [HttpGet("license/{licenseNumber}")]
    public async Task<ActionResult<DoctorResponseDto>> GetDoctorByLicenseNumber(string licenseNumber)
    {
        try
        {
            var doctor = await _doctorService.GetDoctorByLicenseNumberAsync(licenseNumber);
            if (doctor == null)
            {
                return NotFound(new { message = "Doctor not found with this license number" });
            }
            return Ok(doctor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctor with license number {LicenseNumber}", licenseNumber);
            return StatusCode(500, "An error occurred while retrieving doctor details");
        }
    }

    /// <summary>
    /// Get doctors by specialization
    /// </summary>
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

    /// <summary>
    /// Get available doctors
    /// </summary>
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

    /// <summary>
    /// Create doctor details for a user
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<DoctorResponseDto>> CreateDoctor([FromBody] CreateDoctorDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var doctor = await _doctorService.CreateDoctorAsync(dto);
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

    /// <summary>
    /// Update doctor details
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin,Doctor")]
    public async Task<ActionResult<DoctorResponseDto>> UpdateDoctor(int id, [FromBody] UpdateDoctorDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

    /// <summary>
    /// Delete doctor details
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult> DeleteDoctor(int id)
    {
        try
        {
            var result = await _doctorService.DeleteDoctorAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Doctor not found" });
            }
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
