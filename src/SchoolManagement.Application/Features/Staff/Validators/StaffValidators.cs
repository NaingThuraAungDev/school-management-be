using FluentValidation;
using SchoolManagement.Application.Features.Staff.Commands;

namespace SchoolManagement.Application.Features.Staff.Validators;

public class OnboardStaffCommandValidator : AbstractValidator<OnboardStaffCommand>
{
    public OnboardStaffCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

        RuleFor(x => x.JoiningDate)
            .NotEmpty().WithMessage("Joining date is required.");

        RuleFor(x => x.StaffType)
            .IsInEnum().WithMessage("Invalid staff type.");
    }
}
