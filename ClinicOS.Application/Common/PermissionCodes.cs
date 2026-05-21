namespace ClinicOS.Application.Common;

public static class PermissionCodes
{
    public const string ClinicsRead = "clinics.read";
    public const string ClinicsWrite = "clinics.write";

    public const string PatientsRead = "patients.read";
    public const string PatientsWrite = "patients.write";

    public const string AppointmentsRead = "appointments.read";
    public const string AppointmentsWrite = "appointments.write";

    public const string BillingRead = "billing.read";
    public const string BillingWrite = "billing.write";

    public const string UsersRead = "users.read";
    public const string UsersWrite = "users.write";
    public const string UsersManage = "users.manage";

    public const string DoctorsRead = "doctors.read";
    public const string DoctorsWrite = "doctors.write";
    public const string DoctorsManage = "doctors.manage";

    public const string RemindersRead = "reminders.read";
    public const string RemindersManage = "reminders.manage";

    public static readonly IReadOnlyList<string> All =
    [
        ClinicsRead, ClinicsWrite,
        PatientsRead, PatientsWrite,
        AppointmentsRead, AppointmentsWrite,
        BillingRead, BillingWrite,
        UsersRead, UsersWrite, UsersManage,
        DoctorsRead, DoctorsWrite, DoctorsManage,
        RemindersRead, RemindersManage
    ];
}
