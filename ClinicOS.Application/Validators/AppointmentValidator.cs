using ClinicOS.Application.DTOs;
using FluentValidation;

namespace ClinicOS.Application.Validators;

/// <summary>
/// Validator for CreateAppointmentDto
/// </summary>
public class CreateAppointmentValidator : AbstractValidator<CreateAppointmentDto>
{
    public CreateAppointmentValidator()
    {
        RuleFor(x => x.PatientId)
            .GreaterThan(0).WithMessage("Patient ID must be greater than 0");

        RuleFor(x => x.DoctorId)
            .GreaterThan(0).WithMessage("Doctor ID must be greater than 0");

        RuleFor(x => x.AppointmentDate)
            .NotEmpty().WithMessage("Appointment date is required")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Appointment date cannot be in the past");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("End time is required")
            .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time");

        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes cannot exceed 2000 characters");
    }
}

/// <summary>
/// Validator for RescheduleAppointmentDto
/// </summary>
public class RescheduleAppointmentValidator : AbstractValidator<RescheduleAppointmentDto>
{
    public RescheduleAppointmentValidator()
    {
        RuleFor(x => x.NewAppointmentDate)
            .NotEmpty().WithMessage("New appointment date is required")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("New appointment date cannot be in the past");

        RuleFor(x => x.NewStartTime)
            .NotEmpty().WithMessage("New start time is required");

        RuleFor(x => x.NewEndTime)
            .NotEmpty().WithMessage("New end time is required")
            .GreaterThan(x => x.NewStartTime).WithMessage("New end time must be after new start time");
    }
}
