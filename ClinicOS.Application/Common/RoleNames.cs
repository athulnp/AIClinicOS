namespace ClinicOS.Application.Common;

public static class RoleNames
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";
    public const string Doctor = "Doctor";
    public const string Receptionist = "Receptionist";

    public static readonly IReadOnlyList<string> ClinicStaffRoles = [Admin, Doctor, Receptionist];
}
