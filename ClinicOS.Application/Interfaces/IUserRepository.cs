using ClinicOS.Application.DTOs;
using ClinicOS.Domain.Entities;
using ClinicOS.Domain.Enums;

namespace ClinicOS.Application.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username, int? clinicId);
    Task<User?> GetByEmailAsync(string email, int? clinicId);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
    Task<IEnumerable<User>> GetByRoleAsync(UserRole role, int clinicId);
    Task<bool> UsernameExistsAsync(string username, int? clinicId);
    Task<bool> EmailExistsAsync(string email, int? clinicId);
    Task<bool> HasAppointmentsAsync(int userId);
    Task<bool> HasDoctorProfileAsync(int userId);
    Task<IEnumerable<User>> GetPagedAsync(int clinicId, UserListQueryDto query);
    Task<int> GetListCountAsync(int clinicId, UserListQueryDto query);
}
