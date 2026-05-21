using Microsoft.AspNetCore.Authorization;

namespace ClinicOS.API.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(string permissionCode) => PermissionCode = permissionCode;
    public string PermissionCode { get; }
}
