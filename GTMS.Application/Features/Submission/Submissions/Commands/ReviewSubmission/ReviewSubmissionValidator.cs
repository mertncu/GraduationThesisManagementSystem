using FluentValidation;

namespace GTMS.Application.Features.Submission.Submissions.Commands.ReviewSubmission;

public class ReviewSubmissionValidator : AbstractValidator<ReviewSubmissionCommand>
{
    public ReviewSubmissionValidator()
    {
        RuleFor(v => v.SubmissionId)
            .NotEmpty().WithMessage("Submission ID is required.");

        RuleFor(v => v.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(status => new[] { "Approved", "NeedsRevision", "Rejected" }.Contains(status))
            .WithMessage("Invalid status.");

        RuleFor(v => v.Feedback)
            .NotEmpty().When(v => v.Status == "Rejected" || v.Status == "NeedsRevision")
            .WithMessage("Feedback is required when requesting revision or rejecting.")
            .MaximumLength(2000).WithMessage("Feedback must not exceed 2000 characters.");
    }
}
