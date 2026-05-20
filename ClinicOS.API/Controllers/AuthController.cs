using ClinicOS.Application.DTOs;
using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicOS.API.Controllers;

/// <summary>
/// Authentication and user management controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Login endpoint
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result.Data);
    }

    /// <summary>
    /// Refresh token endpoint
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponseDto>> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        var result = await _authService.RefreshTokenAsync(dto);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result.Data);
    }

    /// <summary>
    /// Logout endpoint
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _authService.LogoutAsync(userId);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    /// <summary>
    /// Register new user (Admin only)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] CreateUserDto dto)
    {
        var createdBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        var result = await _authService.RegisterAsync(dto, createdBy);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return CreatedAtAction(nameof(GetUser), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>
    /// Get current user
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _authService.GetCurrentUserAsync(userId);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result.Data);
    }

    /// <summary>
    /// Get user by ID (Admin only)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("users/{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var result = await _authService.GetUserByIdAsync(id);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result.Data);
    }

    /// <summary>
    /// Update user (Admin only)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("users/{id}")]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserDto dto)
    {
        var updatedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        var result = await _authService.UpdateUserAsync(id, dto, updatedBy);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result.Data);
    }

    /// <summary>
    /// Delete user (Admin only)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("users/{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        var deletedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        var result = await _authService.DeleteUserAsync(id, deletedBy);
        if (!result.Success)
        {
            return NotFound(result);
        }
        return Ok(result);
    }
}
