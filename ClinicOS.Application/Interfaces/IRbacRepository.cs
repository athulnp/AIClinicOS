using ClinicOS.Domain.Entities;

namespace ClinicOS.Application.Interfaces;

public interface IRbacRepository
{
    Task SeedRolesAndPermissionsAsync();
    Task<Role?> GetRoleByIdAsync(int roleId);
    Task<Role?> GetRoleByNameAsync(string name);
    Task<IReadOnlyList<Role>> GetAssignableClinicRolesAsync();
    Task<IReadOnlyList<string>> GetPermissionCodesForUserAsync(int userId);
    Task<IReadOnlyList<string>> GetRoleNamesForUserAsync(int userId);
    Task<bool> UserHasRoleAsync(int userId, string roleName);
    Task AssignRoleToUserAsync(int userId, int roleId);
    Task ReplaceUserRoleAsync(int userId, int roleId);
}
