namespace ClinicOS.Domain.Enums;

public enum UserRole
{
    /// <summary>Platform operator — manages clinics across the system.</summary>
    SuperAdmin = 0,
    Admin = 1,
    Doctor = 2,
    Receptionist = 3
}
