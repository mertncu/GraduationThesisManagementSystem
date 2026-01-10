using FluentValidation;

namespace GTMS.Application.Features.Thesis.ThesisProposals.Commands.CreateThesisProposal;

public class CreateThesisProposalValidator : AbstractValidator<CreateThesisProposalCommand>
{
    public CreateThesisProposalValidator()
    {
        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(v => v.Abstract)
            .NotEmpty().WithMessage("Abstract is required.");

        RuleFor(v => v.AdvisorId)
            .NotEmpty().WithMessage("Advisor is required.");
    }
}
