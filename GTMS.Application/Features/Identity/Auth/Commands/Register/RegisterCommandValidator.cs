using FluentValidation;

namespace GTMS.Application.Features.Identity.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(v => v.FirstName)
            .NotEmpty().WithMessage("First Name is required.")
            .MaximumLength(50);
            
        RuleFor(v => v.LastName)
            .NotEmpty().WithMessage("Last Name is required.")
            .MaximumLength(50);

        RuleFor(v => v.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(100);

        RuleFor(v => v.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");
    }
}
