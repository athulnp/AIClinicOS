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

/// <summary>
/// Authentication service implementation
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<CreateUserDto> _createValidator;
    private readonly IValidator<UpdateUserDto> _updateValidator;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        IUserRepository userRepository,
        IValidator<CreateUserDto> createValidator,
        IValidator<UpdateUserDto> updateValidator,
        IConfiguration configuration,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByUsernameAsync(dto.Username);
        if (user == null)
        {
            return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid username or password");
        }

        if (!user.IsActive)
        {
            return ApiResponse<LoginResponseDto>.ErrorResponse("User account is inactive");
        }

        // Verify password (in production, use proper password hashing like BCrypt)
        if (!VerifyPassword(dto.Password, user.PasswordHash))
        {
            return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid username or password");
        }

        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        var userDto = MapToUserDto(user);

        var response = new LoginResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            Expiration = DateTime.UtcNow.AddHours(1),
            User = userDto
        };

        return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful");
    }

    public async Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(RefreshTokenDto dto)
    {
        var user = await _userRepository.GetAllAsync();
        var validUser = user.FirstOrDefault(u => u.RefreshToken == dto.RefreshToken && u.RefreshTokenExpiryTime > DateTime.UtcNow);

        if (validUser == null)
        {
            return ApiResponse<LoginResponseDto>.ErrorResponse("Invalid or expired refresh token");
        }

        var token = GenerateJwtToken(validUser);
        var newRefreshToken = GenerateRefreshToken();

        validUser.RefreshToken = newRefreshToken;
        validUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        _userRepository.Update(validUser);
        await _unitOfWork.SaveChangesAsync();

        var userDto = MapToUserDto(validUser);

        var response = new LoginResponseDto
        {
            Token = token,
            RefreshToken = newRefreshToken,
            Expiration = DateTime.UtcNow.AddHours(1),
            User = userDto
        };

        return ApiResponse<LoginResponseDto>.SuccessResponse(response, "Token refreshed successfully");
    }

    public async Task<ApiResponse> LogoutAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return ApiResponse.ErrorResponse("User not found");
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse.SuccessResponse("Logout successful");
    }

    public async Task<ApiResponse<UserDto>> RegisterAsync(CreateUserDto dto, string createdBy)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return ApiResponse<UserDto>.ErrorResponse("Validation failed", validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        // Check if username already exists
        if (await _userRepository.UsernameExistsAsync(dto.Username))
        {
            return ApiResponse<UserDto>.ErrorResponse("Username already exists");
        }

        // Check if email already exists
        if (await _userRepository.EmailExistsAsync(dto.Email))
        {
            return ApiResponse<UserDto>.ErrorResponse("Email already exists");
        }

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = HashPassword(dto.Password),
            FullName = dto.FullName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Role = dto.Role,
            IsActive = true,
            CreatedBy = createdBy
        };

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var userDto = MapToUserDto(user);
        return ApiResponse<UserDto>.SuccessResponse(userDto, "User created successfully");
    }

    public async Task<ApiResponse<UserDto>> UpdateUserAsync(int id, UpdateUserDto dto, string updatedBy)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return ApiResponse<UserDto>.ErrorResponse("Validation failed", validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return ApiResponse<UserDto>.ErrorResponse("User not found");
        }

        // Check if email is being changed and if it already exists
        if (user.Email != dto.Email && await _userRepository.EmailExistsAsync(dto.Email))
        {
            return ApiResponse<UserDto>.ErrorResponse("Email already exists");
        }

        user.FullName = dto.FullName;
        user.Email = dto.Email;
        user.PhoneNumber = dto.PhoneNumber;
        user.Role = dto.Role;
        user.IsActive = dto.IsActive;
        user.UpdatedBy = updatedBy;

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        var userDto = MapToUserDto(user);
        return ApiResponse<UserDto>.SuccessResponse(userDto, "User updated successfully");
    }

    public async Task<ApiResponse> DeleteUserAsync(int id, string deletedBy)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return ApiResponse.ErrorResponse("User not found");
        }

        _userRepository.Delete(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse.SuccessResponse("User deleted successfully");
    }

    public async Task<ApiResponse<UserDto>> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return ApiResponse<UserDto>.ErrorResponse("User not found");
        }

        var userDto = MapToUserDto(user);
        return ApiResponse<UserDto>.SuccessResponse(userDto);
    }

    public async Task<ApiResponse<UserDto>> GetCurrentUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return ApiResponse<UserDto>.ErrorResponse("User not found");
        }

        var userDto = MapToUserDto(user);
        return ApiResponse<UserDto>.SuccessResponse(userDto);
    }

    private string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "ClinicOS",
            audience: _configuration["Jwt:Audience"] ?? "ClinicOSUsers",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString();
    }

    private string HashPassword(string password)
    {
        // In production, use proper password hashing like BCrypt
        // This is a simple implementation for demonstration
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private bool VerifyPassword(string password, string hash)
    {
        var computedHash = HashPassword(password);
        return computedHash == hash;
    }

    private UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}
