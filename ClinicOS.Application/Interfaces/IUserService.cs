using ClinicOS.Application.Common;
using ClinicOS.Application.DTOs;
using ClinicOS.Domain.Enums;

namespace ClinicOS.Application.Interfaces;

public interface IUserService
{
    Task<ApiResponse<UserDto>> CreateUserAsync(CreateUserDto dto, string createdBy, int? callerClinicId, UserRole callerRole);
    Task<ApiResponse<UserDto>> UpdateUserAsync(int id, UpdateUserDto dto, string updatedBy);
    Task<ApiResponse> DeactivateUserAsync(int id, string deactivatedBy);
    Task<ApiResponse<UserDto>> GetUserByIdAsync(int id);
    Task<ApiResponse<UserDto>> GetCurrentUserAsync(int userId);
    Task<ApiResponse<UserDto>> UpdateProfileAsync(int userId, UpdateProfileDto dto);
    Task<ApiResponse> ChangePasswordAsync(int userId, ChangePasswordDto dto);
    Task<PagedResponse<UserDto>> GetUsersAsync(UserListQueryDto query);
}
