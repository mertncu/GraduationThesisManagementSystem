using FluentValidation;

namespace GTMS.Application.Features.Academic.AcademicTerms.Commands.CreateAcademicTerm;

public class CreateAcademicTermCommandValidator : AbstractValidator<CreateAcademicTermCommand>
{
    public CreateAcademicTermCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(v => v.StartDate)
            .NotEmpty().WithMessage("Start date is required.");

        RuleFor(v => v.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThan(v => v.StartDate).WithMessage("End date must be after start date.");
    }
}
