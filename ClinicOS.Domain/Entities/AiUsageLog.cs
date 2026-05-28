using ClinicOS.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOS.Domain.Entities;

/// <summary>
/// AI usage log entity for tracking AI provider usage and costs
/// </summary>
public class AiUsageLog : AuditableEntity, ITenantEntity
{
    public int ClinicId { get; set; }

    [Required]
    [MaxLength(100)]
    public string FeatureName { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string Provider { get; set; } = null!;

    [MaxLength(100)]
    public string? Model { get; set; }

    public int? PromptTokens { get; set; }

    public int? CompletionTokens { get; set; }

    public int? TotalTokens { get; set; }

    public decimal? EstimatedCost { get; set; }

    public int? UserId { get; set; }

    // Navigation properties
    public virtual Clinic Clinic { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
}
