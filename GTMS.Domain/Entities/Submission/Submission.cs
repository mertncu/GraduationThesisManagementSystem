using GTMS.Domain.Common;
using GTMS.Domain.Entities.Thesis;
using GTMS.Domain.Entities.Identity;
namespace GTMS.Domain.Entities.Submission;

public class Submission : BaseEntity
{
    public Guid ThesisId { get; set; }
    public ThesisProject Thesis { get; set; } = null!;

    public Guid MilestoneId { get; set; }
    public ThesisMilestone Milestone { get; set; } = null!;

    public Guid StudentId { get; set; }
    public User Student { get; set; } = null!;

    public Guid SubmissionStatusId { get; set; }
    public SubmissionStatus SubmissionStatus { get; set; } = null!;
    
    public string? FilePath { get; set; }
    public string? OriginalFileName { get; set; }
    
    public string? Notes { get; set; }
    public string? Feedback { get; set; } // Advisor's feedback
    public DateTime SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public Guid? ReviewedByUserId { get; set; }
}