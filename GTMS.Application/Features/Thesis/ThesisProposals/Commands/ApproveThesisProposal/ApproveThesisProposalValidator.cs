using FluentValidation;

namespace GTMS.Application.Features.Thesis.ThesisProposals.Commands.ApproveThesisProposal;

public class ApproveThesisProposalValidator : AbstractValidator<ApproveThesisProposalCommand>
{
    public ApproveThesisProposalValidator()
    {
        RuleFor(v => v.ThesisId)
            .NotEmpty().WithMessage("Thesis ID is required.");
            
        RuleFor(v => v.Feedback)
            .MaximumLength(1000).WithMessage("Feedback must not exceed 1000 characters.");
    }
}
