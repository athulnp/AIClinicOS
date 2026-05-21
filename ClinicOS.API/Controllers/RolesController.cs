using ClinicOS.Application.Common;
using ClinicOS.Application.DTOs;
using ClinicOS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRbacRepository _rbacRepository;

    public RolesController(IRbacRepository rbacRepository)
    {
        _rbacRepository = rbacRepository;
    }

    /// <summary>Lists clinic-assignable role templates (Admin, Doctor, Receptionist).</summary>
    [Authorize(Policy = PermissionCodes.UsersRead)]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RoleDto>>> GetAssignableRoles()
    {
        var roles = await _rbacRepository.GetAssignableClinicRolesAsync();
        return Ok(roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            IsPlatformRole = r.IsPlatformRole
        }).ToList());
    }
}
