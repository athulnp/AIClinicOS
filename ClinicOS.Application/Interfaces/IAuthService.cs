using ClinicOS.Application.Common;
using ClinicOS.Application.DTOs;

namespace ClinicOS.Application.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto dto);
    Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(RefreshTokenDto dto);
    Task<ApiResponse> LogoutAsync(int userId);
    Task<ApiResponse<UserDto>> GetCurrentUserAsync(int userId);
}
