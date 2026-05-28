namespace ClinicOS.Application.Interfaces;

/// <summary>
/// Interface for AI clinical notes generation services
/// </summary>
public interface IAIClinicalNotesService
{
    /// <summary>
    /// Generates a professional clinical treatment note using AI
    /// </summary>
    /// <param name="procedureType">Type of dental procedure</param>
    /// <param name="toothNumber">Tooth number (if applicable)</param>
    /// <param name="symptoms">Patient symptoms</param>
    /// <param name="diagnosis">Diagnosis</param>
    /// <param name="treatmentPerformed">Treatment performed</param>
    /// <param name="additionalNotes">Additional notes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Generated clinical note</returns>
    Task<string> GenerateTreatmentNoteAsync(
        string procedureType,
        string? toothNumber,
        string? symptoms,
        string? diagnosis,
        string? treatmentPerformed,
        string? additionalNotes,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the provider name for logging purposes
    /// </summary>
    string GetProviderName();
}
