namespace ClinicOS.Domain.Common;

/// <summary>
/// Interface for soft delete support
/// </summary>
public interface ISoftDelete
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
    string? DeletedBy { get; set; }
}
