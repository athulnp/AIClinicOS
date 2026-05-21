using System.Security.Claims;
using ClinicOS.Domain.Entities;

namespace ClinicOS.Application.Common;

public static class UserAuthHelper
{
    public static IReadOnlyList<string> GetRoleNames(User user) =>
        user.UserRoleAssignments.Select(a => a.Role.Name).Distinct().ToList();

    public static IReadOnlyList<string> GetPermissionCodes(User user) =>
        user.UserRoleAssignments
            .SelectMany(a => a.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToList();

    public static bool HasRole(User user, string roleName) =>
        user.UserRoleAssignments.Any(a =>
            string.Equals(a.Role.Name, roleName, StringComparison.OrdinalIgnoreCase));

    public static IEnumerable<Claim> BuildAuthClaims(User user, Clinic? clinic)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email)
        };

        foreach (var roleName in GetRoleNames(user))
            claims.Add(new Claim(ClaimTypes.Role, roleName));

        foreach (var permission in GetPermissionCodes(user))
            claims.Add(new Claim(AuthConstants.PermissionClaimType, permission));

        if (user.ClinicId.HasValue && clinic != null)
        {
            claims.Add(new Claim(TenantConstants.ClinicIdClaim, user.ClinicId.Value.ToString()));
            claims.Add(new Claim(TenantConstants.ClinicCodeClaim, clinic.Code));
        }

        return claims;
    }
}
