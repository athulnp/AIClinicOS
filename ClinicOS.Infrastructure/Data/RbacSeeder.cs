using ClinicOS.Application.Common;
using ClinicOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Infrastructure.Data;

public static class RbacSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await SeedPermissionsAsync(context);
        await SeedRolesAsync(context);
        await context.SaveChangesAsync();
    }

    /// <summary>Moves Users.Role enum column into UserRoleAssignments, then drops the legacy column.</summary>
    public static async Task MigrateFromLegacyRoleColumnAsync(AppDbContext context)
    {
        var legacyColumnLength = await context.Database
            .SqlQueryRaw<int?>("SELECT CAST(COL_LENGTH('Users', 'Role') AS int) AS [Value]")
            .FirstOrDefaultAsync();

        if (!legacyColumnLength.HasValue || legacyColumnLength.Value == 0)
            return;

        await context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO UserRoleAssignments (UserId, RoleId)
            SELECT u.Id, r.Id
            FROM Users u
            INNER JOIN Roles r ON r.Name = CASE u.Role
                WHEN 0 THEN 'SuperAdmin'
                WHEN 1 THEN 'Admin'
                WHEN 2 THEN 'Doctor'
                WHEN 3 THEN 'Receptionist'
                ELSE 'Receptionist'
            END AND r.ClinicId IS NULL
            WHERE NOT EXISTS (
                SELECT 1 FROM UserRoleAssignments ura WHERE ura.UserId = u.Id
            )");

        await context.Database.ExecuteSqlRawAsync(
            "ALTER TABLE Users DROP COLUMN Role");
    }

    public static async Task AssignRoleIfMissingAsync(AppDbContext context, int userId, string roleName)
    {
        var role = await context.Roles.FirstOrDefaultAsync(r => r.Name == roleName && r.ClinicId == null);
        if (role == null)
            return;

        if (await context.UserRoleAssignments.AnyAsync(a => a.UserId == userId && a.RoleId == role.Id))
            return;

        context.UserRoleAssignments.Add(new UserRoleAssignment { UserId = userId, RoleId = role.Id });
        await context.SaveChangesAsync();
    }

    private static async Task SeedPermissionsAsync(AppDbContext context)
    {
        var definitions = new (string Code, string Description, string Module)[]
        {
            (PermissionCodes.ClinicsRead, "View clinics", "Clinics"),
            (PermissionCodes.ClinicsWrite, "Create and update clinics", "Clinics"),
            (PermissionCodes.PatientsRead, "View patients", "Patients"),
            (PermissionCodes.PatientsWrite, "Create and update patients", "Patients"),
            (PermissionCodes.AppointmentsRead, "View appointments", "Appointments"),
            (PermissionCodes.AppointmentsWrite, "Manage appointments", "Appointments"),
            (PermissionCodes.BillingRead, "View billing", "Billing"),
            (PermissionCodes.BillingWrite, "Create billing and record payments", "Billing"),
            (PermissionCodes.UsersRead, "View users", "Users"),
            (PermissionCodes.UsersWrite, "Update users", "Users"),
            (PermissionCodes.UsersManage, "Create and deactivate users", "Users"),
            (PermissionCodes.DoctorsRead, "View doctors", "Doctors"),
            (PermissionCodes.DoctorsWrite, "Update doctor profiles", "Doctors"),
            (PermissionCodes.DoctorsManage, "Create and remove doctor profiles", "Doctors"),
            (PermissionCodes.RemindersRead, "View reminder logs", "Reminders"),
            (PermissionCodes.RemindersManage, "Trigger manual reminders", "Reminders"),
        };

        foreach (var (code, description, module) in definitions)
        {
            if (!await context.Permissions.AnyAsync(p => p.Code == code))
            {
                context.Permissions.Add(new Permission
                {
                    Code = code,
                    Description = description,
                    Module = module
                });
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedRolesAsync(AppDbContext context)
    {
        var permissionMap = await context.Permissions.ToDictionaryAsync(p => p.Code, p => p.Id);

        var roleDefs = new[]
        {
            new
            {
                Name = RoleNames.SuperAdmin,
                Description = "Platform operator — manages all clinics",
                IsPlatformRole = true,
                Permissions = PermissionCodes.All.ToArray()
            },
            new
            {
                Name = RoleNames.Admin,
                Description = "Clinic administrator",
                IsPlatformRole = false,
                Permissions = new[]
                {
                    PermissionCodes.ClinicsRead,
                    PermissionCodes.PatientsRead, PermissionCodes.PatientsWrite,
                    PermissionCodes.AppointmentsRead, PermissionCodes.AppointmentsWrite,
                    PermissionCodes.BillingRead, PermissionCodes.BillingWrite,
                    PermissionCodes.UsersRead, PermissionCodes.UsersWrite, PermissionCodes.UsersManage,
                    PermissionCodes.DoctorsRead, PermissionCodes.DoctorsWrite, PermissionCodes.DoctorsManage,
                    PermissionCodes.RemindersRead, PermissionCodes.RemindersManage
                }
            },
            new
            {
                Name = RoleNames.Doctor,
                Description = "Clinical staff — patients and appointments",
                IsPlatformRole = false,
                Permissions = new[]
                {
                    PermissionCodes.PatientsRead, PermissionCodes.PatientsWrite,
                    PermissionCodes.AppointmentsRead, PermissionCodes.AppointmentsWrite,
                    PermissionCodes.BillingRead,
                    PermissionCodes.DoctorsRead, PermissionCodes.DoctorsWrite,
                    PermissionCodes.RemindersRead
                }
            },
            new
            {
                Name = RoleNames.Receptionist,
                Description = "Front desk — scheduling and patient intake",
                IsPlatformRole = false,
                Permissions = new[]
                {
                    PermissionCodes.PatientsRead, PermissionCodes.PatientsWrite,
                    PermissionCodes.AppointmentsRead, PermissionCodes.AppointmentsWrite,
                    PermissionCodes.BillingRead,
                    PermissionCodes.RemindersRead
                }
            }
        };

        foreach (var def in roleDefs)
        {
            var role = await context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Name == def.Name && r.ClinicId == null);

            if (role == null)
            {
                role = new Role
                {
                    Name = def.Name,
                    Description = def.Description,
                    IsPlatformRole = def.IsPlatformRole,
                    ClinicId = null
                };
                context.Roles.Add(role);
                await context.SaveChangesAsync();
            }
            else
            {
                role.Description = def.Description;
                role.IsPlatformRole = def.IsPlatformRole;
            }

            var desiredPermissionIds = def.Permissions
                .Where(permissionMap.ContainsKey)
                .Select(code => permissionMap[code])
                .ToHashSet();

            var existing = role.RolePermissions.Select(rp => rp.PermissionId).ToHashSet();
            foreach (var permissionId in desiredPermissionIds.Except(existing))
            {
                role.RolePermissions.Add(new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permissionId
                });
            }
        }

        await context.SaveChangesAsync();
    }
}
