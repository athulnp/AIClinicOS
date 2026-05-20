using ClinicOS.Application.Common;
using ClinicOS.Application.DTOs;

namespace ClinicOS.Application.Interfaces;

/// <summary>
/// Authentication service interface
/// </summary>
public interface IAuthService
{
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto dto);
    Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(RefreshTokenDto dto);
    Task<ApiResponse> LogoutAsync(int userId);
    Task<ApiResponse<UserDto>> RegisterAsync(CreateUserDto dto, string createdBy);
    Task<ApiResponse<UserDto>> UpdateUserAsync(int id, UpdateUserDto dto, string updatedBy);
    Task<ApiResponse> DeleteUserAsync(int id, string deletedBy);
    Task<ApiResponse<UserDto>> GetUserByIdAsync(int id);
    Task<ApiResponse<UserDto>> GetCurrentUserAsync(int userId);
}
