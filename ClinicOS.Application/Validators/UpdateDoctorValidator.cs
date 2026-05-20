using FluentValidation;
using ClinicOS.Application.DTOs;

namespace ClinicOS.Application.Validators;

public class UpdateDoctorValidator : AbstractValidator<UpdateDoctorDto>
{
    public UpdateDoctorValidator()
    {
        RuleFor(x => x.Specialization)
            .MaximumLength(100)
            .WithMessage("Specialization cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Specialization));

        RuleFor(x => x.LicenseNumber)
            .MaximumLength(50)
            .WithMessage("License Number cannot exceed 50 characters")
            .Matches(@"^[A-Z0-9\-]+$")
            .WithMessage("License Number must contain only uppercase letters, numbers, and hyphens")
            .When(x => !string.IsNullOrEmpty(x.LicenseNumber));

        RuleFor(x => x.YearsOfExperience)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Years of experience cannot be negative")
            .LessThanOrEqualTo(70)
            .WithMessage("Years of experience seems unrealistic")
            .When(x => x.YearsOfExperience.HasValue);

        RuleFor(x => x.Bio)
            .MaximumLength(500)
            .WithMessage("Bio cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Bio));

        RuleFor(x => x.ConsultationFee)
            .GreaterThan(0)
            .WithMessage("Consultation fee must be greater than 0")
            .When(x => x.ConsultationFee.HasValue && x.ConsultationFee > 0);

        RuleFor(x => x.Department)
            .MaximumLength(100)
            .WithMessage("Department cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Department));

        RuleFor(x => x.ClinicLocation)
            .MaximumLength(200)
            .WithMessage("Clinic Location cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.ClinicLocation));
    }
}
