using ClinicOS.Application.Common;
using ClinicOS.Application.DTOs;
using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITenantContext _tenantContext;

    public UsersController(IUserService userService, ITenantContext tenantContext)
    {
        _userService = userService;
        _tenantContext = tenantContext;
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpGet]
    public async Task<ActionResult<PagedResponse<UserDto>>> GetUsers([FromQuery] UserListQueryDto query)
    {
        if (!_tenantContext.HasClinic)
            return BadRequest(new { message = "Clinic context required. SuperAdmin: send X-Clinic-Id header." });

        return Ok(await _userService.GetUsersAsync(query));
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var result = await _userService.GetUserByIdAsync(id);
        if (!result.Success)
            return NotFound(result);
        return Ok(result.Data);
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto dto)
    {
        var createdBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        var role = Enum.Parse<UserRole>(User.FindFirst(ClaimTypes.Role)?.Value ?? "Admin");
        int? clinicId = int.TryParse(User.FindFirst(TenantConstants.ClinicIdClaim)?.Value, out var cid) ? cid : null;

        var result = await _userService.CreateUserAsync(dto, createdBy, clinicId, role);
        if (!result.Success)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetUser), new { id = result.Data!.Id }, result.Data);
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserDto dto)
    {
        var updatedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        var result = await _userService.UpdateUserAsync(id, dto, updatedBy);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result.Data);
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeactivateUser(int id)
    {
        var deactivatedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        var result = await _userService.DeactivateUserAsync(id, deactivatedBy);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpPut("me")]
    public async Task<ActionResult<UserDto>> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _userService.UpdateProfileAsync(userId, dto);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result.Data);
    }

    [HttpPut("me/password")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _userService.ChangePasswordAsync(userId, dto);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result);
    }
}
