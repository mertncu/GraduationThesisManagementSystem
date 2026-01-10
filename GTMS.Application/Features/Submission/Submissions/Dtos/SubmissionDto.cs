namespace GTMS.Application.Features.Submission.Submissions.Dtos;

public class SubmissionDto
{
    public Guid Id { get; set; }
    public Guid MilestoneId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? FilePath { get; set; }
    public string? OriginalFileName { get; set; }
    public string? Notes { get; set; }
    public string? Feedback { get; set; }
    public DateTime SubmittedAt { get; set; }
}
