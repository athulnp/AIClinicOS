using ClinicOS.Application.DTOs;
using FluentValidation;

namespace ClinicOS.Application.Validators;

/// <summary>
/// Validator for CreatePatientDto
/// </summary>
public class CreatePatientValidator : AbstractValidator<CreatePatientDto>
{
    public CreatePatientValidator()
    {
        RuleFor(x => x.PatientCode)
            .NotEmpty().WithMessage("Patient code is required")
            .MaximumLength(50).WithMessage("Patient code cannot exceed 50 characters");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(200).WithMessage("Full name cannot exceed 200 characters");

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Invalid gender value");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.Now).WithMessage("Date of birth must be in the past");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters")
            .Matches(@"^[0-9+\-\s()]+$").WithMessage("Invalid phone number format");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email address")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Address cannot exceed 500 characters");

        RuleFor(x => x.BloodGroup)
            .MaximumLength(10).WithMessage("Blood group cannot exceed 10 characters");

        RuleFor(x => x.MedicalHistory)
            .MaximumLength(2000).WithMessage("Medical history cannot exceed 2000 characters");

        RuleFor(x => x.Allergies)
            .MaximumLength(1000).WithMessage("Allergies cannot exceed 1000 characters");

        RuleFor(x => x.EmergencyContact)
            .MaximumLength(200).WithMessage("Emergency contact cannot exceed 200 characters");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes cannot exceed 2000 characters");
    }
}

/// <summary>
/// Validator for UpdatePatientDto
/// </summary>
public class UpdatePatientValidator : AbstractValidator<UpdatePatientDto>
{
    public UpdatePatientValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(200).WithMessage("Full name cannot exceed 200 characters");

        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Invalid gender value");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.Now).WithMessage("Date of birth must be in the past");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters")
            .Matches(@"^[0-9+\-\s()]+$").WithMessage("Invalid phone number format");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email address")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Address)
            .MaximumLength(500).WithMessage("Address cannot exceed 500 characters");

        RuleFor(x => x.BloodGroup)
            .MaximumLength(10).WithMessage("Blood group cannot exceed 10 characters");

        RuleFor(x => x.MedicalHistory)
            .MaximumLength(2000).WithMessage("Medical history cannot exceed 2000 characters");

        RuleFor(x => x.Allergies)
            .MaximumLength(1000).WithMessage("Allergies cannot exceed 1000 characters");

        RuleFor(x => x.EmergencyContact)
            .MaximumLength(200).WithMessage("Emergency contact cannot exceed 200 characters");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes cannot exceed 2000 characters");
    }
}
