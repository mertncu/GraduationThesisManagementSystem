using MediatR;

namespace GTMS.Application.Features.Submission.Submissions.Commands.CreateSubmission;

public class CreateSubmissionCommand : IRequest
{
    public Guid ThesisId { get; set; }
    public Guid MilestoneId { get; set; }
    public string? Notes { get; set; }
    
    // File Data
    public Stream? FileContent { get; set; }
    public string? FileName { get; set; }
}
