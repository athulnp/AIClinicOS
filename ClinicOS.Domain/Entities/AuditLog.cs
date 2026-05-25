namespace ClinicOS.Domain.Entities;

/// <summary>
/// Audit log entity for tracking user activities across the system
/// Follows industrial standards for audit trails
/// </summary>
public class AuditLog
{
    public int Id { get; set; }
    public int ClinicId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // CREATE, UPDATE, DELETE, LOGIN, LOGOUT, etc.
    public string EntityType { get; set; } = string.Empty; // Patient, Appointment, Billing, etc.
    public int? EntityId { get; set; }
    public string? EntityName { get; set; } // Human-readable name of the entity
    public string? Description { get; set; } // Detailed description of the action
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? AdditionalData { get; set; } // JSON string for additional context

    // Navigation property
    public Clinic? Clinic { get; set; }
}
