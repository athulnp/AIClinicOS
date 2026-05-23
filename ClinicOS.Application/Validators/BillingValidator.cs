using ClinicOS.Application.DTOs;
using FluentValidation;

namespace ClinicOS.Application.Validators;

/// <summary>
/// Validator for CreateBillingDto
/// </summary>
public class CreateBillingValidator : AbstractValidator<CreateBillingDto>
{
    public CreateBillingValidator()
    {
        RuleFor(x => x.PatientId)
            .GreaterThan(0).WithMessage("Patient ID must be greater than 0");

        RuleFor(x => x.TotalAmount)
            .GreaterThan(0).WithMessage("Total amount must be greater than 0");

        RuleFor(x => x.PaymentMethod)
            .IsInEnum().WithMessage("Invalid payment method");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes cannot exceed 2000 characters");
    }
}

/// <summary>
/// Validator for RecordPaymentDto
/// </summary>
public class RecordPaymentValidator : AbstractValidator<RecordPaymentDto>
{
    public RecordPaymentValidator()
    {
        RuleFor(x => x.PaymentAmount)
            .GreaterThan(0).WithMessage("Payment amount must be greater than 0");

        RuleFor(x => x.PaymentMethod)
            .IsInEnum().WithMessage("Invalid payment method");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes cannot exceed 2000 characters");
    }
}

/// <summary>
/// Validator for UpdateBillingDto
/// </summary>
public class UpdateBillingValidator : AbstractValidator<UpdateBillingDto>
{
    public UpdateBillingValidator()
    {
        RuleFor(x => x.PatientId)
            .GreaterThan(0).WithMessage("Patient ID must be greater than 0");

        RuleFor(x => x.TotalAmount)
            .GreaterThan(0).WithMessage("Total amount must be greater than 0");

        RuleFor(x => x.PaymentMethod)
            .IsInEnum().WithMessage("Invalid payment method");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes cannot exceed 2000 characters");
    }
}
