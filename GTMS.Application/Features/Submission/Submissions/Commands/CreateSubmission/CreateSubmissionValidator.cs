using FluentValidation;

namespace GTMS.Application.Features.Submission.Submissions.Commands.CreateSubmission;

public class CreateSubmissionValidator : AbstractValidator<CreateSubmissionCommand>
{
    public CreateSubmissionValidator()
    {
        RuleFor(v => v.ThesisId)
            .NotEmpty().WithMessage("Thesis ID is required.");

        RuleFor(v => v.MilestoneId)
            .NotEmpty().WithMessage("Milestone ID is required.");

        RuleFor(v => v.Notes)
            .MaximumLength(2000).WithMessage("Notes must not exceed 2000 characters.");
    }
}
