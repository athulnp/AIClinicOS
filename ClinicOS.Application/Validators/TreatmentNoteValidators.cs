using ClinicOS.Application.DTOs;
using FluentValidation;

namespace ClinicOS.Application.Validators;

/// <summary>
/// Validator for GenerateTreatmentNoteRequest
/// </summary>
public class GenerateTreatmentNoteRequestValidator : AbstractValidator<GenerateTreatmentNoteRequest>
{
    public GenerateTreatmentNoteRequestValidator()
    {
        RuleFor(x => x.PatientId)
            .GreaterThan(0).WithMessage("Patient ID is required");

        RuleFor(x => x.AppointmentId)
            .GreaterThan(0).WithMessage("Appointment ID is required");

        RuleFor(x => x.ProcedureType)
            .NotEmpty().WithMessage("Procedure type is required")
            .MaximumLength(200).WithMessage("Procedure type must be less than 200 characters");

        RuleFor(x => x.ToothNumber)
            .MaximumLength(10).WithMessage("Tooth number must be less than 10 characters")
            .When(x => !string.IsNullOrEmpty(x.ToothNumber));

        RuleFor(x => x.Symptoms)
            .MaximumLength(1000).WithMessage("Symptoms must be less than 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Symptoms));

        RuleFor(x => x.Diagnosis)
            .MaximumLength(1000).WithMessage("Diagnosis must be less than 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Diagnosis));

        RuleFor(x => x.TreatmentPerformed)
            .MaximumLength(2000).WithMessage("Treatment performed must be less than 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.TreatmentPerformed));

        RuleFor(x => x.AdditionalNotes)
            .MaximumLength(1000).WithMessage("Additional notes must be less than 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.AdditionalNotes));
    }
}

/// <summary>
/// Validator for CreateTreatmentNoteDto
/// </summary>
public class CreateTreatmentNoteDtoValidator : AbstractValidator<CreateTreatmentNoteDto>
{
    public CreateTreatmentNoteDtoValidator()
    {
        RuleFor(x => x.PatientId)
            .GreaterThan(0).WithMessage("Patient ID is required");

        RuleFor(x => x.AppointmentId)
            .GreaterThan(0).WithMessage("Appointment ID is required");

        RuleFor(x => x.ProcedureType)
            .NotEmpty().WithMessage("Procedure type is required")
            .MaximumLength(200).WithMessage("Procedure type must be less than 200 characters");

        RuleFor(x => x.ToothNumber)
            .MaximumLength(10).WithMessage("Tooth number must be less than 10 characters")
            .When(x => !string.IsNullOrEmpty(x.ToothNumber));

        RuleFor(x => x.Symptoms)
            .MaximumLength(1000).WithMessage("Symptoms must be less than 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Symptoms));

        RuleFor(x => x.Diagnosis)
            .MaximumLength(1000).WithMessage("Diagnosis must be less than 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Diagnosis));

        RuleFor(x => x.TreatmentPerformed)
            .MaximumLength(2000).WithMessage("Treatment performed must be less than 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.TreatmentPerformed));

        RuleFor(x => x.AdditionalNotes)
            .MaximumLength(1000).WithMessage("Additional notes must be less than 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.AdditionalNotes));

        RuleFor(x => x.AiGeneratedNote)
            .MaximumLength(5000).WithMessage("AI generated note must be less than 5000 characters")
            .When(x => !string.IsNullOrEmpty(x.AiGeneratedNote));

        RuleFor(x => x.FinalNote)
            .MaximumLength(5000).WithMessage("Final note must be less than 5000 characters")
            .When(x => !string.IsNullOrEmpty(x.FinalNote));
    }
}

/// <summary>
/// Validator for UpdateTreatmentNoteDto
/// </summary>
public class UpdateTreatmentNoteDtoValidator : AbstractValidator<UpdateTreatmentNoteDto>
{
    public UpdateTreatmentNoteDtoValidator()
    {
        RuleFor(x => x.ProcedureType)
            .NotEmpty().WithMessage("Procedure type is required")
            .MaximumLength(200).WithMessage("Procedure type must be less than 200 characters")
            .When(x => !string.IsNullOrEmpty(x.ProcedureType));

        RuleFor(x => x.ToothNumber)
            .MaximumLength(10).WithMessage("Tooth number must be less than 10 characters")
            .When(x => !string.IsNullOrEmpty(x.ToothNumber));

        RuleFor(x => x.Symptoms)
            .MaximumLength(1000).WithMessage("Symptoms must be less than 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Symptoms));

        RuleFor(x => x.Diagnosis)
            .MaximumLength(1000).WithMessage("Diagnosis must be less than 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Diagnosis));

        RuleFor(x => x.TreatmentPerformed)
            .MaximumLength(2000).WithMessage("Treatment performed must be less than 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.TreatmentPerformed));

        RuleFor(x => x.AdditionalNotes)
            .MaximumLength(1000).WithMessage("Additional notes must be less than 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.AdditionalNotes));

        RuleFor(x => x.FinalNote)
            .MaximumLength(5000).WithMessage("Final note must be less than 5000 characters")
            .When(x => !string.IsNullOrEmpty(x.FinalNote));
    }
}
