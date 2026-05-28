namespace ClinicOS.Application.Services;

/// <summary>
/// Reusable prompt templates for AI clinical notes generation
/// </summary>
public static class PromptTemplates
{
    /// <summary>
    /// Generates a prompt for creating professional dental treatment notes
    /// </summary>
    public static string GenerateTreatmentNotePrompt(
        string procedureType,
        string? toothNumber,
        string? symptoms,
        string? diagnosis,
        string? treatmentPerformed,
        string? additionalNotes)
    {
        var prompt = @"You are assisting dentists in generating professional clinical treatment notes.

Generate a comprehensive and professional dental clinical note based on the following information using the EXACT template format shown below:

IMPORTANT GUIDELINES:
- Do NOT invent or hallucinate any medical information
- Use ONLY the supplied details below
- Use professional dental terminology
- Structure the note EXACTLY as shown in the template below
- Do not include speculative information
- Do not include information not provided in the input
- If a section has no relevant information, use ""Not applicable"" or leave blank

PROCEDURE DETAILS:
- Procedure Type: {ProcedureType}
- Tooth Number: {ToothNumber}
- Symptoms: {Symptoms}
- Diagnosis: {Diagnosis}
- Treatment Performed: {TreatmentPerformed}
- Additional Notes: {AdditionalNotes}

TEMPLATE FORMAT (follow this EXACT structure):

**Chief Complaint:**
[Patient's reported symptoms or reason for visit based on provided symptoms]

**History of Present Illness:**
[Duration, onset, characteristics of symptoms based on provided information]
[Previous treatments or medications if mentioned]

**Clinical Examination:**
- Tooth #: {ToothNumber}
- Visual inspection: [Findings from diagnosis/treatment details]
- Percussion test: [If mentioned in symptoms/treatment]
- Mobility: [If mentioned]
- Periodontal status: [If mentioned]
- Radiographic findings: [If mentioned in diagnosis]

**Diagnosis:**
[Primary diagnosis from provided diagnosis field]
[Secondary diagnosis if applicable]

**Treatment Plan:**
[Procedure performed from treatment performed field]
[Materials used if mentioned]
[Technique details if mentioned]
[Anesthesia used if mentioned]

**Post-operative Instructions:**
[Specific instructions for patient based on procedure type]
[Follow-up recommendations based on procedure type]

**Prognosis:**
[Expected outcome based on procedure type]
[Potential complications if relevant to procedure]

Generate the clinical note following the EXACT template structure above.";

        return prompt
            .Replace("{ProcedureType}", procedureType)
            .Replace("{ToothNumber}", toothNumber ?? "N/A")
            .Replace("{Symptoms}", symptoms ?? "N/A")
            .Replace("{Diagnosis}", diagnosis ?? "N/A")
            .Replace("{TreatmentPerformed}", treatmentPerformed ?? "N/A")
            .Replace("{AdditionalNotes}", additionalNotes ?? "N/A");
    }

    /// <summary>
    /// System prompt for setting AI behavior
    /// </summary>
    public static string SystemPrompt => @"You are a dental clinical documentation assistant. Your role is to help dentists generate professional, concise, and accurate clinical treatment notes.

IMPORTANT SAFETY RULES:
1. Never invent or hallucinate medical information
2. Only use information explicitly provided in the input
3. Use professional dental terminology
4. Keep notes concise and clinically relevant
5. Follow standard clinical documentation formats
6. Do not include speculative or uncertain information
7. If information is missing, do not fill in assumptions

Your output will be reviewed and potentially edited by the dentist before being saved as the final clinical record.";
}
