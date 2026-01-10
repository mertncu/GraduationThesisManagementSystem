using FluentValidation;

namespace GTMS.Application.Features.Thesis.ThesisMilestones.Commands.CreateThesisMilestone;

public class CreateThesisMilestoneValidator : AbstractValidator<CreateThesisMilestoneCommand>
{
    public CreateThesisMilestoneValidator()
    {
        RuleFor(v => v.ThesisId)
            .NotEmpty().WithMessage("Thesis ID is required.");

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
            
        RuleFor(v => v.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(v => v.DueDate)
            .NotEmpty().WithMessage("Due Date is required.")
            .GreaterThan(DateTime.UtcNow).WithMessage("Due Date must be in the future.");
    }
}
