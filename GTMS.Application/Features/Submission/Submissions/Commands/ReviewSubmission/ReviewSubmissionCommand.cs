using MediatR;

namespace GTMS.Application.Features.Submission.Submissions.Commands.ReviewSubmission;

public record ReviewSubmissionCommand : IRequest<Unit>
{
    public Guid SubmissionId { get; init; }
    public string Status { get; init; } = null!; // "Approved", "NeedsRevision"
    public string? Feedback { get; init; }
}
