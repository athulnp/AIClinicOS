using ClinicOS.Application.Common;
using ClinicOS.Application.DTOs;
using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Entities;
using FluentValidation;

namespace ClinicOS.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IClinicRepository _clinicRepository;
    private readonly IRbacRepository _rbacRepository;
    private readonly ITenantContext _tenantContext;
    private readonly IValidator<CreateUserDto> _createValidator;
    private readonly IValidator<UpdateUserDto> _updateValidator;
    private readonly IValidator<UpdateProfileDto> _profileValidator;
    private readonly IValidator<ChangePasswordDto> _passwordValidator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogService _auditLogService;

    public UserService(
        IUserRepository userRepository,
        IClinicRepository clinicRepository,
        IRbacRepository rbacRepository,
        ITenantContext tenantContext,
        IValidator<CreateUserDto> createValidator,
        IValidator<UpdateUserDto> updateValidator,
        IValidator<UpdateProfileDto> profileValidator,
        IValidator<ChangePasswordDto> passwordValidator,
        IUnitOfWork unitOfWork,
        IAuditLogService auditLogService)
    {
        _userRepository = userRepository;
        _clinicRepository = clinicRepository;
        _rbacRepository = rbacRepository;
        _tenantContext = tenantContext;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _profileValidator = profileValidator;
        _passwordValidator = passwordValidator;
        _unitOfWork = unitOfWork;
        _auditLogService = auditLogService;
    }

    public async Task<ApiResponse<UserDto>> CreateUserAsync(
        CreateUserDto dto, string createdBy, int userId, int? callerClinicId)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ApiResponse<UserDto>.ErrorResponse("Validation failed",
                validation.Errors.Select(e => e.ErrorMessage).ToList());

        var role = await _rbacRepository.GetRoleByIdAsync(dto.RoleId);
        if (role == null)
            return ApiResponse<UserDto>.ErrorResponse("Invalid role");

        if (role.IsPlatformRole)
            return ApiResponse<UserDto>.ErrorResponse("Cannot assign platform roles via this endpoint");

        var clinicId = ResolveTargetClinicId(dto.ClinicId, callerClinicId);
        if (!clinicId.HasValue)
            return ApiResponse<UserDto>.ErrorResponse("Clinic context is required to create a user");

        if (await _userRepository.UsernameExistsAsync(dto.Username, clinicId))
            return ApiResponse<UserDto>.ErrorResponse("Username already exists in this clinic");

        if (await _userRepository.EmailExistsAsync(dto.Email, clinicId))
            return ApiResponse<UserDto>.ErrorResponse("Email already exists in this clinic");

        var user = new User
        {
            ClinicId = clinicId,
            Username = dto.Username,
            PasswordHash = PasswordHasher.Hash(dto.Password),
            FullName = dto.FullName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            IsActive = true,
            CreatedBy = createdBy
        };

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        await _rbacRepository.AssignRoleToUserAsync(user.Id, role.Id);

        // Log audit activity
        if (clinicId.HasValue)
        {
            await _auditLogService.LogActivityAsync(
                clinicId.Value,
                userId,
                createdBy,
                "CREATE",
                "User",
                user.Id,
                user.FullName,
                $"Created new user: {user.FullName} with role {role.Name}"
            );
        }

        var loaded = await _userRepository.GetByIdAsync(user.Id);
        return ApiResponse<UserDto>.SuccessResponse(MapToUserDto(loaded!), "User created successfully");
    }

    public async Task<ApiResponse<UserDto>> UpdateUserAsync(int id, UpdateUserDto dto, string updatedBy, int userId)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ApiResponse<UserDto>.ErrorResponse("Validation failed",
                validation.Errors.Select(e => e.ErrorMessage).ToList());

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return ApiResponse<UserDto>.ErrorResponse("User not found");

        if (await _rbacRepository.UserHasRoleAsync(id, RoleNames.SuperAdmin))
            return ApiResponse<UserDto>.ErrorResponse("Cannot modify SuperAdmin account");

        var role = await _rbacRepository.GetRoleByIdAsync(dto.RoleId);
        if (role == null)
            return ApiResponse<UserDto>.ErrorResponse("Invalid role");

        if (role.IsPlatformRole)
            return ApiResponse<UserDto>.ErrorResponse("Cannot assign platform roles");

        if (user.Email != dto.Email && await _userRepository.EmailExistsAsync(dto.Email, user.ClinicId))
            return ApiResponse<UserDto>.ErrorResponse("Email already exists in this clinic");

        user.FullName = dto.FullName;
        user.Email = dto.Email;
        user.PhoneNumber = dto.PhoneNumber;
        user.IsActive = dto.IsActive;
        user.UpdatedBy = updatedBy;

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        await _rbacRepository.ReplaceUserRoleAsync(id, role.Id);

        // Log audit activity
        if (user.ClinicId.HasValue)
        {
            await _auditLogService.LogActivityAsync(
                user.ClinicId.Value,
                userId,
                updatedBy,
                "UPDATE",
                "User",
                user.Id,
                user.FullName,
                $"Updated user: {user.FullName} with role {role.Name}"
            );
        }

        var loaded = await _userRepository.GetByIdAsync(id);
        return ApiResponse<UserDto>.SuccessResponse(MapToUserDto(loaded!), "User updated successfully");
    }

    public async Task<ApiResponse> DeactivateUserAsync(int id, string deactivatedBy, int userId)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return ApiResponse.ErrorResponse("User not found");

        if (await _rbacRepository.UserHasRoleAsync(id, RoleNames.SuperAdmin))
            return ApiResponse.ErrorResponse("Cannot deactivate SuperAdmin account");

        if (await _userRepository.HasAppointmentsAsync(id))
            return ApiResponse.ErrorResponse(
                "Cannot delete user with existing appointments. Deactivate the account instead.",
                new List<string> { "Set isActive to false via PUT /api/users/{id}" });

        if (await _userRepository.HasDoctorProfileAsync(id))
            return ApiResponse.ErrorResponse(
                "Cannot delete user with a doctor profile. Remove the doctor profile first or deactivate the account.");

        user.IsActive = false;
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        user.UpdatedBy = deactivatedBy;
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        // Log audit activity
        if (user.ClinicId.HasValue)
        {
            await _auditLogService.LogActivityAsync(
                user.ClinicId.Value,
                userId,
                deactivatedBy,
                "DEACTIVATE",
                "User",
                user.Id,
                user.FullName,
                $"Deactivated user: {user.FullName}"
            );
        }

        return ApiResponse.SuccessResponse("User deactivated successfully");
    }

    public async Task<ApiResponse> ActivateUserAsync(int id, string activatedBy, int userId)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return ApiResponse.ErrorResponse("User not found");

        if (user.IsActive)
            return ApiResponse.ErrorResponse("User is already active");

        user.IsActive = true;
        user.UpdatedBy = activatedBy;
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        // Log audit activity
        if (user.ClinicId.HasValue)
        {
            await _auditLogService.LogActivityAsync(
                user.ClinicId.Value,
                userId,
                activatedBy,
                "ACTIVATE",
                "User",
                user.Id,
                user.FullName,
                $"Activated user: {user.FullName}"
            );
        }

        return ApiResponse.SuccessResponse("User activated successfully");
    }

    public async Task<ApiResponse<UserDto>> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return ApiResponse<UserDto>.ErrorResponse("User not found");
        return ApiResponse<UserDto>.SuccessResponse(MapToUserDto(user));
    }

    public async Task<ApiResponse<UserDto>> GetCurrentUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return ApiResponse<UserDto>.ErrorResponse("User not found");
        return ApiResponse<UserDto>.SuccessResponse(MapToUserDto(user));
    }

    public async Task<ApiResponse<UserDto>> UpdateProfileAsync(int userId, UpdateProfileDto dto)
    {
        var validation = await _profileValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ApiResponse<UserDto>.ErrorResponse("Validation failed",
                validation.Errors.Select(e => e.ErrorMessage).ToList());

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return ApiResponse<UserDto>.ErrorResponse("User not found");

        if (user.Email != dto.Email && await _userRepository.EmailExistsAsync(dto.Email, user.ClinicId))
            return ApiResponse<UserDto>.ErrorResponse("Email already exists");

        user.FullName = dto.FullName;
        user.Email = dto.Email;
        user.PhoneNumber = dto.PhoneNumber;

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<UserDto>.SuccessResponse(MapToUserDto(user), "Profile updated successfully");
    }

    public async Task<ApiResponse> ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        var validation = await _passwordValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ApiResponse.ErrorResponse("Validation failed",
                validation.Errors.Select(e => e.ErrorMessage).ToList());

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return ApiResponse.ErrorResponse("User not found");

        if (!PasswordHasher.Verify(dto.CurrentPassword, user.PasswordHash))
            return ApiResponse.ErrorResponse("Current password is incorrect");

        user.PasswordHash = PasswordHasher.Hash(dto.NewPassword);
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse.SuccessResponse("Password changed successfully");
    }

    public async Task<PagedResponse<UserDto>> GetUsersAsync(UserListQueryDto query)
    {
        if (!_tenantContext.HasClinic)
            return PagedResponse<UserDto>.Create(new List<UserDto>(), query.PageNumber, query.PageSize, 0);

        var clinicId = _tenantContext.ClinicId!.Value;
        var users = await _userRepository.GetPagedAsync(clinicId, query);
        var total = await _userRepository.GetListCountAsync(clinicId, query);

        return PagedResponse<UserDto>.Create(users.Select(MapToUserDto).ToList(), query.PageNumber, query.PageSize, total);
    }

    private int? ResolveTargetClinicId(int? dtoClinicId, int? callerClinicId)
    {
        if (_tenantContext.IsSuperAdmin)
            return dtoClinicId ?? _tenantContext.ClinicId;

        return callerClinicId ?? _tenantContext.ClinicId;
    }

    private static UserDto MapToUserDto(User user)
    {
        var primaryRole = user.UserRoleAssignments.FirstOrDefault()?.Role;
        return new UserDto
        {
            Id = user.Id,
            ClinicId = user.ClinicId,
            ClinicName = user.Clinic?.Name,
            Username = user.Username,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            RoleId = primaryRole?.Id ?? 0,
            RoleName = primaryRole?.Name ?? string.Empty,
            Permissions = UserAuthHelper.GetPermissionCodes(user),
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}
