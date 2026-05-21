using ClinicOS.Application.Common;
using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Entities;
using ClinicOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Infrastructure.Repositories;

public class RbacRepository : IRbacRepository
{
    private readonly AppDbContext _context;

    public RbacRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task SeedRolesAndPermissionsAsync() => RbacSeeder.SeedAsync(_context);

    public async Task<Role?> GetRoleByIdAsync(int roleId) =>
        await _context.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == roleId);

    public async Task<Role?> GetRoleByNameAsync(string name) =>
        await _context.Roles.FirstOrDefaultAsync(r => r.Name == name && r.ClinicId == null);

    public async Task<IReadOnlyList<Role>> GetAssignableClinicRolesAsync() =>
        await _context.Roles
            .Where(r => !r.IsPlatformRole && r.ClinicId == null)
            .OrderBy(r => r.Name)
            .ToListAsync();

    public async Task<IReadOnlyList<string>> GetPermissionCodesForUserAsync(int userId) =>
        await _context.UserRoleAssignments
            .Where(a => a.UserId == userId)
            .SelectMany(a => a.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToListAsync();

    public async Task<IReadOnlyList<string>> GetRoleNamesForUserAsync(int userId) =>
        await _context.UserRoleAssignments
            .Where(a => a.UserId == userId)
            .Select(a => a.Role.Name)
            .Distinct()
            .ToListAsync();

    public async Task<bool> UserHasRoleAsync(int userId, string roleName) =>
        await _context.UserRoleAssignments.AnyAsync(a =>
            a.UserId == userId &&
            a.Role.Name == roleName);

    public async Task AssignRoleToUserAsync(int userId, int roleId)
    {
        if (await _context.UserRoleAssignments.AnyAsync(a => a.UserId == userId && a.RoleId == roleId))
            return;

        _context.UserRoleAssignments.Add(new UserRoleAssignment { UserId = userId, RoleId = roleId });
        await _context.SaveChangesAsync();
    }

    public async Task ReplaceUserRoleAsync(int userId, int roleId)
    {
        var existing = await _context.UserRoleAssignments.Where(a => a.UserId == userId).ToListAsync();
        _context.UserRoleAssignments.RemoveRange(existing);
        _context.UserRoleAssignments.Add(new UserRoleAssignment { UserId = userId, RoleId = roleId });
        await _context.SaveChangesAsync();
    }
}
