using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ClinicOS.Application.Common;
using ClinicOS.Application.DTOs;
using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Entities;
using ClinicOS.Domain.Enums;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ClinicOS.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IClinicRepository _clinicRepository;
    private readonly IValidator<LoginDto> _loginValidator;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        IUserRepository userRepository,
        IClinicRepository clinicRepository,
        IValidator<LoginDto> loginValidator,
        IConfiguration configuration,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _clinicRepository = clinicRepository;
        _loginValidator = loginValidator;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto dto)
    {
        var validation = await _loginValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return ApiResponse<LoginResponseDto>.ErrorResponse("Validation failed",
                validation.Errors.Select(e => e.ErrorMessage).ToList());

        User? user;
        Clinic? clinic = null;

        var hasClinicContext = dto.ClinicId.HasValue || !string.IsNullOrWhiteSpace(dto.ClinicCode);

        if (!hasClinicContext)
        {
            user = await _userRepository.GetByUsernameAsync(dto.Username, null);
            if (user == null || user.Role != UserRole.SuperAdmin)
                return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid username or password");
        }
        else
        {
            clinic = await ResolveClinicAsync(dto);
            if (clinic == null || !clinic.IsActive)
                return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid clinic");

            user = await _userRepository.GetByUsernameAsync(dto.Username, clinic.Id);
            if (user == null)
                return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid username or password");
        }

        if (!user.IsActive)
            return ApiResponse<LoginResponseDto>.ErrorResponse("User account is inactive");

        if (!PasswordHasher.Verify(dto.Password, user.PasswordHash, out var needsRehash))
            return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid username or password");

        if (needsRehash)
        {
            user.PasswordHash = PasswordHasher.Hash(dto.Password);
            _userRepository.Update(user);
        }

        if (user.ClinicId.HasValue)
        {
            clinic ??= await _clinicRepository.GetByIdAsync(user.ClinicId.Value);
            if (clinic == null || !clinic.IsActive)
                return ApiResponse<LoginResponseDto>.ErrorResponse("Clinic is inactive");
        }

        var token = GenerateJwtToken(user, clinic);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<LoginResponseDto>.SuccessResponse(new LoginResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            Expiration = DateTime.UtcNow.AddHours(1),
            User = MapToUserDto(user, clinic)
        }, "Login successful");
    }

    public async Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(RefreshTokenDto dto)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(dto.RefreshToken);
        if (user == null)
            return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid or expired refresh token");

        Clinic? clinic = null;
        if (user.ClinicId.HasValue)
            clinic = await _clinicRepository.GetByIdAsync(user.ClinicId.Value);

        var token = GenerateJwtToken(user, clinic);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<LoginResponseDto>.SuccessResponse(new LoginResponseDto
        {
            Token = token,
            RefreshToken = newRefreshToken,
            Expiration = DateTime.UtcNow.AddHours(1),
            User = MapToUserDto(user, clinic)
        }, "Token refreshed successfully");
    }

    public async Task<ApiResponse> LogoutAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return ApiResponse.ErrorResponse("User not found");

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse.SuccessResponse("Logout successful");
    }

    public async Task<ApiResponse<UserDto>> GetCurrentUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return ApiResponse<UserDto>.ErrorResponse("User not found");

        Clinic? clinic = null;
        if (user.ClinicId.HasValue)
            clinic = await _clinicRepository.GetByIdAsync(user.ClinicId.Value);

        return ApiResponse<UserDto>.SuccessResponse(MapToUserDto(user, clinic));
    }

    private async Task<Clinic?> ResolveClinicAsync(LoginDto dto)
    {
        if (dto.ClinicId.HasValue)
            return await _clinicRepository.GetByIdAsync(dto.ClinicId.Value);

        return await _clinicRepository.GetByCodeAsync(dto.ClinicCode!.Trim().ToLowerInvariant());
    }

    private string GenerateJwtToken(User user, Clinic? clinic)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        if (user.ClinicId.HasValue && clinic != null)
        {
            claims.Add(new Claim(TenantConstants.ClinicIdClaim, user.ClinicId.Value.ToString()));
            claims.Add(new Claim(TenantConstants.ClinicCodeClaim, clinic.Code));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "ClinicOS",
            audience: _configuration["Jwt:Audience"] ?? "ClinicOSUsers",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken() =>
        Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString();

    private static UserDto MapToUserDto(User user, Clinic? clinic) => new()
    {
        Id = user.Id,
        ClinicId = user.ClinicId,
        ClinicName = clinic?.Name,
        Username = user.Username,
        FullName = user.FullName,
        Email = user.Email,
        PhoneNumber = user.PhoneNumber,
        Role = user.Role,
        IsActive = user.IsActive,
        CreatedAt = user.CreatedAt
    };
}
