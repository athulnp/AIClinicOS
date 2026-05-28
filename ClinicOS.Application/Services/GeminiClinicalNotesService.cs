using ClinicOS.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClinicOS.Application.Services;

/// <summary>
/// Gemini API implementation for AI clinical notes generation
/// </summary>
public class GeminiClinicalNotesService : IAIClinicalNotesService
{
    private readonly HttpClient _httpClient;
    private readonly GeminiOptions _options;
    private readonly ILogger<GeminiClinicalNotesService> _logger;
    private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/";

    public GeminiClinicalNotesService(
        HttpClient httpClient,
        IOptions<GeminiOptions> options,
        ILogger<GeminiClinicalNotesService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> GenerateTreatmentNoteAsync(
        string procedureType,
        string? toothNumber,
        string? symptoms,
        string? diagnosis,
        string? treatmentPerformed,
        string? additionalNotes,
        CancellationToken cancellationToken = default)
    {
        var prompt = PromptTemplates.GenerateTreatmentNotePrompt(
            procedureType,
            toothNumber,
            symptoms,
            diagnosis,
            treatmentPerformed,
            additionalNotes);

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = PromptTemplates.SystemPrompt + "\n\n" + prompt
                        }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 0.3,
                maxOutputTokens = 1000
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var url = $"{BaseUrl}{_options.Model}:generateContent?key={_options.ApiKey}";
        
        _logger.LogInformation("Sending request to Gemini API for treatment note generation");

        try
        {
            var response = await _httpClient.PostAsync(url, content, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Gemini API returned error: {StatusCode} - {Response}", response.StatusCode, errorResponse);
                throw new InvalidOperationException($"Gemini API error: {response.StatusCode} - {errorResponse}");
            }

            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<GeminiResponse>(responseBody);

            if (result?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text != null)
            {
                _logger.LogInformation("Successfully generated treatment note from Gemini API");
                return result.Candidates[0].Content.Parts[0].Text;
            }

            _logger.LogError("Invalid response structure from Gemini API: {Response}", responseBody);
            throw new InvalidOperationException("Invalid response from Gemini API");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating treatment note from Gemini API");
            throw;
        }
    }

    public string GetProviderName() => "Gemini";
}

/// <summary>
/// Gemini configuration options
/// </summary>
public class GeminiOptions
{
    public const string SectionName = "AI:Gemini";
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gemini-1.5-flash";
}

/// <summary>
/// Gemini API response models
/// </summary>
internal class GeminiResponse
{
    [JsonPropertyName("candidates")]
    public List<GeminiCandidate>? Candidates { get; set; }

    [JsonPropertyName("usageMetadata")]
    public GeminiUsageMetadata? UsageMetadata { get; set; }
}

internal class GeminiCandidate
{
    [JsonPropertyName("content")]
    public GeminiContent? Content { get; set; }

    [JsonPropertyName("finishReason")]
    public string? FinishReason { get; set; }

    [JsonPropertyName("index")]
    public int Index { get; set; }
}

internal class GeminiContent
{
    [JsonPropertyName("parts")]
    public List<GeminiPart>? Parts { get; set; }

    [JsonPropertyName("role")]
    public string? Role { get; set; }
}

internal class GeminiPart
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }
}

internal class GeminiUsageMetadata
{
    [JsonPropertyName("promptTokenCount")]
    public int PromptTokenCount { get; set; }

    [JsonPropertyName("candidatesTokenCount")]
    public int CandidatesTokenCount { get; set; }

    [JsonPropertyName("totalTokenCount")]
    public int TotalTokenCount { get; set; }

    [JsonPropertyName("promptTokensDetails")]
    public List<GeminiTokenDetail>? PromptTokensDetails { get; set; }

    [JsonPropertyName("thoughtsTokenCount")]
    public int? ThoughtsTokenCount { get; set; }

    [JsonPropertyName("serviceTier")]
    public string? ServiceTier { get; set; }
}

internal class GeminiTokenDetail
{
    [JsonPropertyName("modality")]
    public string? Modality { get; set; }

    [JsonPropertyName("tokenCount")]
    public int TokenCount { get; set; }
}
