using ClinicOS.Application.Common;
using Microsoft.AspNetCore.Authorization;

namespace ClinicOS.API.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.HasClaim(AuthConstants.PermissionClaimType, requirement.PermissionCode))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
