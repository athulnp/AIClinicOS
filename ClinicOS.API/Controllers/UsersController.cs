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
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITenantContext _tenantContext;

    public UsersController(IUserService userService, ITenantContext tenantContext)
    {
        _userService = userService;
        _tenantContext = tenantContext;
    }

    [Authorize(Policy = PermissionCodes.UsersRead)]
    [HttpGet]
    public async Task<ActionResult<PagedResponse<UserDto>>> GetUsers([FromQuery] UserListQueryDto query)
    {
        if (!_tenantContext.HasClinic)
            return BadRequest(new { message = "Clinic context required. SuperAdmin: send X-Clinic-Id header." });

        return Ok(await _userService.GetUsersAsync(query));
    }

    [Authorize(Policy = PermissionCodes.UsersRead)]
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var result = await _userService.GetUserByIdAsync(id);
        if (!result.Success)
            return NotFound(result);
        return Ok(result.Data);
    }

    [Authorize(Policy = PermissionCodes.UsersManage)]
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto dto)
    {
        var createdBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        
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
            clinicId = int.TryParse(User.FindFirst(TenantConstants.ClinicIdClaim)?.Value, out var cid) ? cid : null;
        }

        var result = await _userService.CreateUserAsync(dto, createdBy, clinicId);
        if (!result.Success)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetUser), new { id = result.Data!.Id }, result.Data);
    }

    [Authorize(Policy = PermissionCodes.UsersManage)]
    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserDto dto)
    {
        var updatedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        var result = await _userService.UpdateUserAsync(id, dto, updatedBy);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result.Data);
    }

    [Authorize(Policy = PermissionCodes.UsersManage)]
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
