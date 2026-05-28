using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ClinicOS.Application.DTOs;

/// <summary>
/// DTO for generating AI treatment note
/// </summary>
public class GenerateTreatmentNoteRequest
{
    [Required]
    public int PatientId { get; set; }

    [Required]
    public int AppointmentId { get; set; }

    [Required]
    [MaxLength(200)]
    public string ProcedureType { get; set; } = null!;

    [MaxLength(10)]
    public string? ToothNumber { get; set; }

    [MaxLength(1000)]
    public string? Symptoms { get; set; }

    [MaxLength(1000)]
    public string? Diagnosis { get; set; }

    [MaxLength(2000)]
    public string? TreatmentPerformed { get; set; }

    [MaxLength(1000)]
    public string? AdditionalNotes { get; set; }
}

/// <summary>
/// DTO for AI-generated treatment note response
/// </summary>
public class GenerateTreatmentNoteResponse
{
    [JsonPropertyName("generatedNote")]
    public string GeneratedNote { get; set; } = string.Empty;
}

/// <summary>
/// DTO for creating a treatment note
/// </summary>
public class CreateTreatmentNoteDto
{
    [Required]
    public int PatientId { get; set; }

    [Required]
    public int AppointmentId { get; set; }

    [Required]
    [MaxLength(200)]
    public string ProcedureType { get; set; } = null!;

    [MaxLength(10)]
    public string? ToothNumber { get; set; }

    [MaxLength(1000)]
    public string? Symptoms { get; set; }

    [MaxLength(1000)]
    public string? Diagnosis { get; set; }

    [MaxLength(2000)]
    public string? TreatmentPerformed { get; set; }

    [MaxLength(1000)]
    public string? AdditionalNotes { get; set; }

    [MaxLength(5000)]
    [JsonPropertyName("aiGeneratedNote")]
    public string? AiGeneratedNote { get; set; }

    [MaxLength(5000)]
    [JsonPropertyName("finalNote")]
    public string? FinalNote { get; set; }
}

/// <summary>
/// DTO for updating a treatment note
/// </summary>
public class UpdateTreatmentNoteDto
{
    [MaxLength(200)]
    public string? ProcedureType { get; set; }

    [MaxLength(10)]
    public string? ToothNumber { get; set; }

    [MaxLength(1000)]
    public string? Symptoms { get; set; }

    [MaxLength(1000)]
    public string? Diagnosis { get; set; }

    [MaxLength(2000)]
    public string? TreatmentPerformed { get; set; }

    [MaxLength(1000)]
    public string? AdditionalNotes { get; set; }

    [MaxLength(5000)]
    [JsonPropertyName("finalNote")]
    public string? FinalNote { get; set; }
}

/// <summary>
/// DTO for treatment note response
/// </summary>
public class TreatmentNoteDto
{
    public int Id { get; set; }
    public int ClinicId { get; set; }
    public int PatientId { get; set; }
    public int AppointmentId { get; set; }
    public string ProcedureType { get; set; } = string.Empty;
    public string? ToothNumber { get; set; }
    public string? Symptoms { get; set; }
    public string? Diagnosis { get; set; }
    public string? TreatmentPerformed { get; set; }
    public string? AdditionalNotes { get; set; }
    [JsonPropertyName("aiGeneratedNote")]
    public string? AiGeneratedNote { get; set; }
    [JsonPropertyName("finalNote")]
    public string? FinalNote { get; set; }
    public int GeneratedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    [JsonPropertyName("generatedByUserName")]
    public string? GeneratedByUserName { get; set; }
    [JsonPropertyName("patientName")]
    public string? PatientName { get; set; }
}
